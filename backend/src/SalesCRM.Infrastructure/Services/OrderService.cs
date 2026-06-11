using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Interfaces.Repositories;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly IVnPayService _vnPayService;
    private readonly IMoMoService _moMoService;
    private readonly IVietQrService _vietQrService;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public OrderService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        IVnPayService vnPayService,
        IMoMoService moMoService,
        IVietQrService vietQrService,
        IConfiguration configuration,
        IStringLocalizer<SharedResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _context = context;
        _vnPayService = vnPayService;
        _moMoService = moMoService;
        _vietQrService = vietQrService;
        _configuration = configuration;
        _localizer = localizer;
    }

    public async Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Resolve Branch ID
        var branchId = _currentUserService.BranchId;
        if (!branchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            if (defaultBranch == null)
            {
                return ApiResult<OrderDto>.Failure("No branch exists in the system.");
            }
            branchId = defaultBranch.Id;
        }

        // 2. Verify Customer if provided
        if (dto.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(dto.CustomerId.Value);
            if (customer == null)
            {
                return ApiResult<OrderDto>.Failure(_localizer["CustomerNotFound", dto.CustomerId.Value]);
            }
        }

        // 2b. Verify Active Shift
        var isStaff = _currentUserService.Role == "Staff";
        var currentUserId = string.IsNullOrEmpty(_currentUserService.UserId) ? Guid.Empty : Guid.Parse(_currentUserService.UserId);
        var activeShift = await _context.Shifts
            .FirstOrDefaultAsync(s => s.UserId == currentUserId && s.BranchId == branchId.Value && s.Status == "Open", cancellationToken);

        if (activeShift == null && isStaff)
        {
            return ApiResult<OrderDto>.Failure("Ca làm việc chưa được mở. Vui lòng mở ca trước khi bán hàng.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var totalAmount = 0m;
            var orderDetails = new List<OrderDetail>();

            // 3. Deduct Stock & Calculate Amounts
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResult<OrderDto>.Failure(_localizer["ProductNotFound", item.ProductId]);
                }

                // Check Inventory
                var inventories = await _unitOfWork.Repository<BranchInventory>()
                    .FindAsync(i => i.BranchId == branchId.Value && i.ProductId == item.ProductId);
                var inventory = inventories.FirstOrDefault();

                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    var available = inventory?.Quantity ?? 0;
                    return ApiResult<OrderDto>.Failure(_localizer["OrderInsufficientStock", product.Name, available, item.Quantity]);
                }

                // Deduct from batch
                if (item.ProductBatchId.HasValue)
                {
                    var batch = await _unitOfWork.Repository<ProductBatch>().GetByIdAsync(item.ProductBatchId.Value);
                    if (batch == null || batch.BranchId != branchId.Value || batch.ProductId != item.ProductId)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResult<OrderDto>.Failure(_localizer["OrderProductBatchNotFound", item.ProductBatchId.Value]);
                    }

                    if (batch.Quantity < item.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResult<OrderDto>.Failure(_localizer["OrderProductBatchInsufficient", batch.BatchCode, batch.Quantity, item.Quantity]);
                    }

                    batch.Quantity -= item.Quantity;
                    _unitOfWork.Repository<ProductBatch>().Update(batch);
                }
                else
                {
                    // FEIFO auto-deduction (First Expired First Out)
                    var batchesRaw = await _unitOfWork.Repository<ProductBatch>()
                        .FindAsync(b => b.BranchId == branchId.Value && b.ProductId == item.ProductId && b.Quantity > 0);
                    var batches = batchesRaw.OrderBy(b => b.ExpiryDate).ToList();

                    var remainingToDeduct = item.Quantity;
                    foreach (var batch in batches)
                    {
                        if (remainingToDeduct <= 0) break;
                        var deductAmount = Math.Min(batch.Quantity, remainingToDeduct);
                        batch.Quantity -= deductAmount;
                        remainingToDeduct -= deductAmount;
                        _unitOfWork.Repository<ProductBatch>().Update(batch);
                    }

                    // Note: If there's still remaining stock to deduct but no batches are found/enough, 
                    // we still proceed as BranchInventory had enough stock (could happen if inventory sync gets mismatched).
                }

                // Decrement branch inventory
                inventory.Quantity -= item.Quantity;
                _unitOfWork.Repository<BranchInventory>().Update(inventory);

                // Decrement shelf stock (ProductLocation) if it exists
                var shelfStocks = await _context.ProductLocations
                    .Where(pl => pl.BranchId == branchId.Value && pl.ProductId == item.ProductId && pl.Quantity > 0)
                    .OrderBy(pl => pl.Quantity)
                    .ToListAsync(cancellationToken);

                var remainingToDeductShelf = item.Quantity;
                foreach (var shelf in shelfStocks)
                {
                    if (remainingToDeductShelf <= 0) break;
                    var deductAmount = Math.Min(shelf.Quantity, remainingToDeductShelf);
                    shelf.Quantity -= deductAmount;
                    remainingToDeductShelf -= deductAmount;
                    _unitOfWork.Repository<ProductLocation>().Update(shelf);
                }

                // Add order detail
                var lineTotal = item.Quantity * product.SellingPrice;
                totalAmount += lineTotal;

                orderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductBatchId = item.ProductBatchId,
                    Quantity = item.Quantity,
                    UnitPrice = product.SellingPrice
                });
            }

            var finalAmount = Math.Max(0, totalAmount - dto.Discount);
            var orderCode = "ORD" + DateTime.UtcNow.ToString("yyMMddHHmmss") + new Random().Next(100, 999);

            var order = new Order
            {
                BranchId = branchId.Value,
                OrderCode = orderCode,
                CustomerId = dto.CustomerId,
                TotalAmount = totalAmount,
                Discount = dto.Discount,
                FinalAmount = finalAmount,
                Status = OrderStatus.Draft,
                ShiftId = activeShift?.Id,
                CouponCode = dto.CouponCode,
                OrderDetails = orderDetails
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Handle immediate payment if specified
            if (dto.PaymentMethod.HasValue && dto.PaymentAmount.HasValue && dto.PaymentAmount.Value > 0)
            {
                var payAmount = Math.Min(dto.PaymentAmount.Value, finalAmount);
                var payment = new Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = dto.PaymentMethod.Value,
                    Amount = payAmount,
                    Status = dto.PaymentMethod.Value == PaymentMethod.Cash ? PaymentStatus.Completed : PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Payment>().AddAsync(payment);
                
                if (dto.PaymentMethod.Value == PaymentMethod.Cash && payAmount >= finalAmount)
                {
                    order.Status = OrderStatus.Paid;
                    _unitOfWork.Repository<Order>().Update(order);
                }
                else if (dto.PaymentMethod.Value == PaymentMethod.Cash)
                {
                    order.Status = OrderStatus.Confirmed;
                    _unitOfWork.Repository<Order>().Update(order);
                }

                if (payment.Status == PaymentStatus.Completed && activeShift != null)
                {
                    if (payment.PaymentMethod == PaymentMethod.Cash)
                        activeShift.TotalSalesCash += payAmount;
                    else if (payment.PaymentMethod == PaymentMethod.MoMo)
                        activeShift.TotalSalesMomo += payAmount;
                    else if (payment.PaymentMethod == PaymentMethod.VNPAY)
                        activeShift.TotalSalesVNPay += payAmount;
                    else if (payment.PaymentMethod == PaymentMethod.BankTransfer)
                        activeShift.TotalSalesBank += payAmount;

                    _context.Shifts.Update(activeShift);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Map and return OrderDto
            var resultDto = await MapToOrderDtoAsync(order, cancellationToken);
            return ApiResult<OrderDto>.Success(resultDto, _localizer["OrderCreateSuccess"]);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResult<OrderDto>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult<OrderDto>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var orderQuery = _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(d => d.Product)
            .Include(o => o.Payments)
            .AsNoTracking();

        var order = await orderQuery.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (order == null)
        {
            return ApiResult<OrderDto>.Failure(_localizer["OrderNotFound", id]);
        }

        // Branch isolation
        if (!_currentUserService.IsAdmin && order.BranchId != _currentUserService.BranchId)
        {
            return ApiResult<OrderDto>.Failure(_localizer["OrderNotFound", id]);
        }

        var dto = await MapToOrderDtoAsync(order, cancellationToken);
        return ApiResult<OrderDto>.Success(dto);
    }

    public async Task<ApiResult<PagedResult<OrderDto>>> GetPagedOrdersAsync(
        string? searchTerm, 
        Guid? branchId, 
        OrderStatus? status, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(d => d.Product)
            .Include(o => o.Payments)
            .AsNoTracking()
            .AsQueryable();

        // Enforce branch boundary
        if (!_currentUserService.IsAdmin)
        {
            var myBranchId = _currentUserService.BranchId;
            query = query.Where(o => o.BranchId == myBranchId);
        }
        else if (branchId.HasValue)
        {
            query = query.Where(o => o.BranchId == branchId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(o => o.OrderCode.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtoItems = new List<OrderDto>();
        foreach (var order in items)
        {
            dtoItems.Add(await MapToOrderDtoAsync(order, cancellationToken));
        }

        var pagedResult = new PagedResult<OrderDto>(dtoItems, pageIndex, pageSize, totalCount);
        return ApiResult<PagedResult<OrderDto>>.Success(pagedResult);
    }

    public async Task<ApiResult<PaymentResponseDto>> ProcessOrderPaymentAsync(Guid orderId, PaymentMethod method, decimal amount, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            return ApiResult<PaymentResponseDto>.Failure(_localizer["OrderNotFound", orderId]);
        }

        if (!_currentUserService.IsAdmin && order.BranchId != _currentUserService.BranchId)
        {
            return ApiResult<PaymentResponseDto>.Failure(_localizer["OrderNotFound", orderId]);
        }

        if (amount <= 0)
        {
            return ApiResult<PaymentResponseDto>.Failure(_localizer["PaymentAmountInvalid"]);
        }

        var totalPaid = order.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        if (totalPaid >= order.FinalAmount)
        {
            return ApiResult<PaymentResponseDto>.Failure(_localizer["PaymentOrderAlreadyPaid"]);
        }

        var remainingAmount = order.FinalAmount - totalPaid;
        var payAmount = Math.Min(amount, remainingAmount);

        var payment = new Payment
        {
            OrderId = orderId,
            PaymentMethod = method,
            Amount = payAmount,
            Status = method == PaymentMethod.Cash ? PaymentStatus.Completed : PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Payment>().AddAsync(payment);

        string? paymentUrl = null;
        string? qrPayload = null;

        if (method == PaymentMethod.Cash)
        {
            // Cash completes instantly
            if (totalPaid + payAmount >= order.FinalAmount)
            {
                order.Status = OrderStatus.Paid;
            }
            else
            {
                order.Status = OrderStatus.Confirmed;
            }
            _unitOfWork.Repository<Order>().Update(order);
        }
        else if (method == PaymentMethod.BankTransfer)
        {
            var vietQrConfig = _configuration.GetSection("VietQR");
            var bankBin = vietQrConfig["BankBin"] ?? "970436"; // Vietcombank default
            var acctNo = vietQrConfig["AccountNumber"] ?? "1234567890";
            var acctName = vietQrConfig["AccountName"] ?? "CONG TY SALESCRM";
            
            paymentUrl = _vietQrService.GenerateQrUrl(bankBin, acctNo, acctName, payAmount, $"Thanh toan don hang {order.OrderCode}");
            qrPayload = paymentUrl;
        }
        else if (method == PaymentMethod.VNPAY)
        {
            // Dummy or client request IP
            paymentUrl = await _vnPayService.CreatePaymentUrlAsync(orderId, payAmount, "127.0.0.1", cancellationToken);
        }
        else if (method == PaymentMethod.MoMo)
        {
            paymentUrl = await _moMoService.CreatePaymentUrlAsync(orderId, payAmount, cancellationToken);
        }

        if (payment.Status == PaymentStatus.Completed && order.ShiftId.HasValue)
        {
            var shift = await _context.Shifts.FindAsync(new object[] { order.ShiftId.Value }, cancellationToken);
            if (shift != null && shift.Status == "Open")
            {
                if (payment.PaymentMethod == PaymentMethod.Cash)
                    shift.TotalSalesCash += payAmount;
                else if (payment.PaymentMethod == PaymentMethod.MoMo)
                    shift.TotalSalesMomo += payAmount;
                else if (payment.PaymentMethod == PaymentMethod.VNPAY)
                    shift.TotalSalesVNPay += payAmount;
                else if (payment.PaymentMethod == PaymentMethod.BankTransfer)
                    shift.TotalSalesBank += payAmount;
                
                _context.Shifts.Update(shift);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new PaymentResponseDto
        {
            PaymentId = payment.Id,
            OrderId = orderId,
            PaymentUrl = paymentUrl,
            VietQrPayload = qrPayload,
            IsCompleted = payment.Status == PaymentStatus.Completed,
            Message = _localizer["PaymentCreateSuccess"]
        };

        return ApiResult<PaymentResponseDto>.Success(response, _localizer["PaymentCreateSuccess"]);
    }

    private async Task<OrderDto> MapToOrderDtoAsync(Order order, CancellationToken cancellationToken)
    {
        var branch = await _context.Branches.FindAsync(new object[] { order.BranchId }, cancellationToken);
        
        string? customerName = null;
        if (order.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(new object[] { order.CustomerId.Value }, cancellationToken);
            customerName = customer?.Name;
        }

        var details = new List<OrderDetailDto>();
        foreach (var d in order.OrderDetails)
        {
            string? batchCode = null;
            if (d.ProductBatchId.HasValue)
            {
                var batch = await _context.ProductBatches.FindAsync(new object[] { d.ProductBatchId.Value }, cancellationToken);
                batchCode = batch?.BatchCode;
            }

            details.Add(new OrderDetailDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductCode = d.Product?.Code,
                ProductName = d.Product?.Name,
                ProductBatchId = d.ProductBatchId,
                BatchCode = batchCode,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                SubTotal = d.SubTotal
            });
        }

        var payments = order.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount,
            Status = p.Status,
            TransactionNo = p.TransactionNo,
            CreatedAt = p.CreatedAt
        }).ToList();

        return new OrderDto
        {
            Id = order.Id,
            BranchId = order.BranchId,
            BranchName = branch?.Name,
            OrderCode = order.OrderCode,
            CustomerId = order.CustomerId,
            CustomerName = customerName,
            TotalAmount = order.TotalAmount,
            Discount = order.Discount,
            FinalAmount = order.FinalAmount,
            Status = order.Status,
            CouponCode = order.CouponCode,
            CreatedAt = order.CreatedAt,
            OrderDetails = details,
            Payments = payments
        };
    }
}
