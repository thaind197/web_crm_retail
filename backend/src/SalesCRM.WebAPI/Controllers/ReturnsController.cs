using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReturnsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ReturnsController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> CreateReturn([FromBody] CreateReturnRequest model)
    {
        if (string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return BadRequest(ApiResult<Guid>.Failure("User ID is not resolved."));
        }

        var userId = Guid.Parse(_currentUserService.UserId);
        var branchId = _currentUserService.BranchId;

        if (!branchId.HasValue)
        {
            return BadRequest(ApiResult<Guid>.Failure("Branch ID is not resolved."));
        }

        if (model.Items == null || model.Items.Count == 0)
        {
            return BadRequest(ApiResult<Guid>.Failure("Danh sách sản phẩm trả lại không được trống."));
        }

        // 1. Get original order
        var order = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == model.OriginalOrderId);

        if (order == null)
        {
            return NotFound(ApiResult<Guid>.Failure("Không tìm thấy hóa đơn gốc."));
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return BadRequest(ApiResult<Guid>.Failure("Hóa đơn này đã bị hủy, không thể trả hàng."));
        }

        // 2. Fetch existing returns for this order
        var existingReturns = await _dbContext.ReturnOrders
            .Include(r => r.ReturnItems)
            .Where(r => r.OriginalOrderId == model.OriginalOrderId)
            .ToListAsync();

        var alreadyReturnedQty = existingReturns
            .SelectMany(r => r.ReturnItems)
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        // Start transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var returnCode = "RET" + DateTime.UtcNow.ToString("yyMMddHHmmss");
            decimal totalRefundAmount = 0;
            var returnItems = new List<ReturnOrderItem>();

            foreach (var item in model.Items)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(d => d.ProductId == item.ProductId);
                if (orderDetail == null)
                {
                    return BadRequest(ApiResult<Guid>.Failure($"Sản phẩm không có trong đơn hàng gốc."));
                }

                int alreadyReturned = alreadyReturnedQty.ContainsKey(item.ProductId) ? alreadyReturnedQty[item.ProductId] : 0;
                int maxReturnable = orderDetail.Quantity - alreadyReturned;

                if (item.Quantity <= 0)
                {
                    return BadRequest(ApiResult<Guid>.Failure("Số lượng trả lại phải lớn hơn 0."));
                }

                if (item.Quantity > maxReturnable)
                {
                    return BadRequest(ApiResult<Guid>.Failure($"Số lượng sản phẩm trả lại ({item.Quantity}) vượt quá số lượng có thể trả ({maxReturnable})."));
                }

                decimal itemRefundAmount = orderDetail.UnitPrice * item.Quantity;
                totalRefundAmount += itemRefundAmount;

                var returnItem = new ReturnOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    RefundPrice = orderDetail.UnitPrice
                };
                returnItems.Add(returnItem);

                // 3. Update BranchInventory
                var inventory = await _dbContext.BranchInventories
                    .FirstOrDefaultAsync(i => i.BranchId == order.BranchId && i.ProductId == item.ProductId);

                if (inventory != null)
                {
                    inventory.Quantity += item.Quantity;
                    _dbContext.BranchInventories.Update(inventory);
                }

                // 4. Update ProductBatch if applicable
                if (orderDetail.ProductBatchId.HasValue)
                {
                    var batch = await _dbContext.ProductBatches.FindAsync(orderDetail.ProductBatchId.Value);
                    if (batch != null)
                    {
                        batch.Quantity += item.Quantity;
                        _dbContext.ProductBatches.Update(batch);
                    }
                }

                // 5. Create InventoryTransaction Log
                var invTx = new InventoryTransaction
                {
                    BranchId = order.BranchId,
                    ProductId = item.ProductId,
                    ProductBatchId = orderDetail.ProductBatchId,
                    Quantity = item.Quantity, // Inflow back to store
                    Type = TransactionType.Import,
                    ReferenceCode = returnCode,
                    Notes = $"Khách hàng trả hàng cho đơn {order.OrderCode}"
                };
                _dbContext.InventoryTransactions.Add(invTx);
            }

            // Cap the total refund to avoid exceeding final amount paid
            var originalPaid = order.Payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
            var previousRefunded = existingReturns.Sum(r => r.RefundAmount);
            var maxRefundableCash = originalPaid - previousRefunded;

            if (totalRefundAmount > maxRefundableCash)
            {
                totalRefundAmount = maxRefundableCash;
            }

            var returnOrder = new ReturnOrder
            {
                BranchId = order.BranchId,
                OriginalOrderId = order.Id,
                ReturnCode = returnCode,
                RefundAmount = totalRefundAmount,
                RefundMethod = model.RefundMethod,
                Reason = model.Reason,
                ReturnItems = returnItems
            };

            _dbContext.ReturnOrders.Add(returnOrder);

            // 6. Subtract from active shift if refunding with cash
            if (model.RefundMethod == "Cash")
            {
                var activeShift = await _dbContext.Shifts
                    .FirstOrDefaultAsync(s => s.UserId == userId && s.BranchId == branchId.Value && s.Status == "Open");

                if (activeShift != null)
                {
                    activeShift.TotalSalesCash -= totalRefundAmount;
                    _dbContext.Shifts.Update(activeShift);
                }
            }

            // 7. Check if all items in original order are fully returned, if so, update status
            int totalItemsInOrder = order.OrderDetails.Sum(d => d.Quantity);
            int totalReturnedSoFar = alreadyReturnedQty.Values.Sum() + model.Items.Sum(i => i.Quantity);

            if (totalReturnedSoFar >= totalItemsInOrder)
            {
                order.Status = OrderStatus.Cancelled; // Or we could keep Paid but fully refunded, Cancelled is simpler.
                _dbContext.Orders.Update(order);
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(ApiResult<Guid>.Success(returnOrder.Id, "Tạo đơn trả hàng và hoàn tiền thành công."));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ApiResult<Guid>.Failure(ex.Message));
        }
    }
}

public class CreateReturnRequest
{
    public Guid OriginalOrderId { get; set; }
    public required string RefundMethod { get; set; } // "Cash", "Momo", "VNPay"
    public string? Reason { get; set; }
    public List<CreateReturnItemRequest> Items { get; set; } = [];
}

public class CreateReturnItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
