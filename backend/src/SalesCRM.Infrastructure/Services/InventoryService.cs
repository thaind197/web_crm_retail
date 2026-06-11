using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Repositories;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public InventoryService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ApplicationDbContext context,
        IStringLocalizer<SharedResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _context = context;
        _localizer = localizer;
    }

    public async Task<ApiResult<Guid>> BatchImportAsync(Guid branchId, List<BatchImportItemDto> items, CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary
        if (!_currentUserService.IsAdmin && branchId != _currentUserService.BranchId)
        {
            return ApiResult<Guid>.Failure("Unauthorized to import to this branch.");
        }

        var branch = await _context.Branches.FindAsync(new object[] { branchId }, cancellationToken);
        if (branch == null)
        {
            return ApiResult<Guid>.Failure(_localizer["BranchNotFound", branchId]);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var importCode = "IMP" + DateTime.UtcNow.ToString("yyMMddHHmmss");

            foreach (var item in items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResult<Guid>.Failure(_localizer["ProductNotFound", item.ProductId]);
                }

                // 1. Update BranchInventory
                var inventories = await _unitOfWork.Repository<BranchInventory>()
                    .FindAsync(i => i.BranchId == branchId && i.ProductId == item.ProductId);
                var inventory = inventories.FirstOrDefault();

                if (inventory == null)
                {
                    inventory = new BranchInventory
                    {
                        BranchId = branchId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        MinimumStockLevel = item.MinimumStockLevel > 0 ? item.MinimumStockLevel : 10
                    };
                    await _unitOfWork.Repository<BranchInventory>().AddAsync(inventory);
                }
                else
                {
                    inventory.Quantity += item.Quantity;
                    if (item.MinimumStockLevel > 0)
                    {
                        inventory.MinimumStockLevel = item.MinimumStockLevel;
                    }
                    _unitOfWork.Repository<BranchInventory>().Update(inventory);
                }

                // 2. Create ProductBatch
                var batch = new ProductBatch
                {
                    BranchId = branchId,
                    ProductId = item.ProductId,
                    BatchCode = item.BatchCode.Trim(),
                    ExpiryDate = item.ExpiryDate,
                    ManufacturedDate = item.ManufacturedDate,
                    Quantity = item.Quantity
                };
                await _unitOfWork.Repository<ProductBatch>().AddAsync(batch);

                // 3. Create Transaction Log
                var transaction = new InventoryTransaction
                {
                    BranchId = branchId,
                    ProductId = item.ProductId,
                    ProductBatchId = batch.Id,
                    Quantity = item.Quantity,
                    Type = TransactionType.Import,
                    ReferenceCode = importCode,
                    Notes = $"Nhập kho theo lô {item.BatchCode}"
                };
                await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResult<Guid>.Success(branchId, _localizer["InventoryImportSuccess"]);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResult<Guid>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult<PagedResult<NearExpiryProductDto>>> GetNearExpiryProductsAsync(
        Guid? branchId, 
        int thresholdDays, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary
        Guid? targetBranchId = _currentUserService.IsAdmin ? branchId : _currentUserService.BranchId;
        if (!targetBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            targetBranchId = defaultBranch?.Id;
        }

        var expiryThreshold = DateTime.UtcNow.AddDays(thresholdDays);

        var query = _context.ProductBatches
            .Include(b => b.Product)
            .Where(b => b.BranchId == targetBranchId && b.Quantity > 0 && b.ExpiryDate <= expiryThreshold)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.ExpiryDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(b => new NearExpiryProductDto
        {
            ProductId = b.ProductId,
            ProductCode = b.Product?.Code,
            ProductName = b.Product?.Name,
            BatchId = b.Id,
            BatchCode = b.BatchCode,
            ExpiryDate = b.ExpiryDate,
            DaysRemaining = (b.ExpiryDate - DateTime.UtcNow).Days,
            Quantity = b.Quantity
        }).ToList();

        var pagedResult = new PagedResult<NearExpiryProductDto>(dtos, pageIndex, pageSize, totalCount);
        return ApiResult<PagedResult<NearExpiryProductDto>>.Success(pagedResult);
    }

    public async Task<ApiResult<PagedResult<LowStockProductDto>>> GetLowStockProductsAsync(
        Guid? branchId, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary
        Guid? targetBranchId = _currentUserService.IsAdmin ? branchId : _currentUserService.BranchId;
        if (!targetBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            targetBranchId = defaultBranch?.Id;
        }

        var query = _context.BranchInventories
            .Include(i => i.Product)
            .Where(i => i.BranchId == targetBranchId && i.Quantity < i.MinimumStockLevel)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.Quantity)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(i => new LowStockProductDto
        {
            ProductId = i.ProductId,
            ProductCode = i.Product?.Code,
            ProductName = i.Product?.Name,
            CurrentQuantity = i.Quantity,
            MinimumStockLevel = i.MinimumStockLevel
        }).ToList();

        var pagedResult = new PagedResult<LowStockProductDto>(dtos, pageIndex, pageSize, totalCount);
        return ApiResult<PagedResult<LowStockProductDto>>.Success(pagedResult);
    }

    public async Task<ApiResult<Guid>> CreateStockTransferAsync(CreateStockTransferDto dto, CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary for source
        Guid? fromBranchId = _currentUserService.BranchId;
        if (!fromBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            if (defaultBranch == null)
            {
                return ApiResult<Guid>.Failure("No default branch exists in the system.");
            }
            fromBranchId = defaultBranch.Id;
        }

        Guid fromBranchIdValue = fromBranchId.Value;

        if (fromBranchIdValue == dto.ToBranchId)
        {
            return ApiResult<Guid>.Failure("Source branch and destination branch must be different.");
        }

        var toBranch = await _context.Branches.FindAsync(new object[] { dto.ToBranchId }, cancellationToken);
        if (toBranch == null)
        {
            return ApiResult<Guid>.Failure(_localizer["BranchNotFound", dto.ToBranchId]);
        }

        if (dto.Items == null || dto.Items.Count == 0)
        {
            return ApiResult<Guid>.Failure(_localizer["StockTransferDetailNotFound"]);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var transferCode = "TRF" + DateTime.UtcNow.ToString("yyMMddHHmmss") + new Random().Next(100, 999);
            var details = new List<StockTransferDetail>();

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResult<Guid>.Failure(_localizer["ProductNotFound", item.ProductId]);
                }

                // Verify stock at source
                var inventories = await _unitOfWork.Repository<BranchInventory>()
                    .FindAsync(i => i.BranchId == fromBranchIdValue && i.ProductId == item.ProductId);
                var inventory = inventories.FirstOrDefault();

                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    var available = inventory?.Quantity ?? 0;
                    return ApiResult<Guid>.Failure(_localizer["OrderInsufficientStock", product.Name, available, item.Quantity]);
                }

                Guid? finalBatchId = item.ProductBatchId;

                // Deduct from batch
                if (item.ProductBatchId.HasValue)
                {
                    var batch = await _unitOfWork.Repository<ProductBatch>().GetByIdAsync(item.ProductBatchId.Value);
                    if (batch == null || batch.BranchId != fromBranchIdValue || batch.ProductId != item.ProductId)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResult<Guid>.Failure(_localizer["OrderProductBatchNotFound", item.ProductBatchId.Value]);
                    }

                    if (batch.Quantity < item.Quantity)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResult<Guid>.Failure(_localizer["OrderProductBatchInsufficient", batch.BatchCode, batch.Quantity, item.Quantity]);
                    }

                    batch.Quantity -= item.Quantity;
                    _unitOfWork.Repository<ProductBatch>().Update(batch);
                }
                else
                {
                    // FEIFO Sequential deduction
                    var batchesRaw = await _unitOfWork.Repository<ProductBatch>()
                        .FindAsync(b => b.BranchId == fromBranchIdValue && b.ProductId == item.ProductId && b.Quantity > 0);
                    var batches = batchesRaw.OrderBy(b => b.ExpiryDate).ToList();

                    var remainingToDeduct = item.Quantity;
                    foreach (var batch in batches)
                    {
                        if (remainingToDeduct <= 0) break;
                        var deductAmount = Math.Min(batch.Quantity, remainingToDeduct);
                        batch.Quantity -= deductAmount;
                        remainingToDeduct -= deductAmount;
                        _unitOfWork.Repository<ProductBatch>().Update(batch);

                        if (!finalBatchId.HasValue)
                        {
                            finalBatchId = batch.Id; // Link first deducted batch as reference
                        }
                    }
                }

                // Decrement source inventory
                inventory.Quantity -= item.Quantity;
                _unitOfWork.Repository<BranchInventory>().Update(inventory);

                // Add detail
                details.Add(new StockTransferDetail
                {
                    ProductId = item.ProductId,
                    ProductBatchId = finalBatchId,
                    Quantity = item.Quantity
                });

                // Create Transaction log at source
                var transaction = new InventoryTransaction
                {
                    BranchId = fromBranchIdValue,
                    ProductId = item.ProductId,
                    ProductBatchId = finalBatchId,
                    Quantity = -item.Quantity, // Outflow
                    Type = TransactionType.TransferOut,
                    ReferenceCode = transferCode,
                    Notes = $"Chuyển đi chi nhánh khác ({toBranch.Name})"
                };
                await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
            }

            var transfer = new StockTransfer
            {
                TransferCode = transferCode,
                FromBranchId = fromBranchIdValue,
                ToBranchId = dto.ToBranchId,
                Status = TransferStatus.Shipped, // Auto shipped on creation
                Notes = dto.Notes,
                TransferDetails = details
            };

            await _unitOfWork.Repository<StockTransfer>().AddAsync(transfer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResult<Guid>.Success(transfer.Id, _localizer["StockTransferCreateSuccess"]);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResult<Guid>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult> ReceiveStockTransferAsync(Guid transferId, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.StockTransfers
            .Include(t => t.TransferDetails)
            .FirstOrDefaultAsync(t => t.Id == transferId, cancellationToken);

        if (transfer == null)
        {
            return ApiResult.Failure(_localizer["StockTransferNotFound", transferId]);
        }

        // Enforce boundary: only destination branch users can receive it
        if (!_currentUserService.IsAdmin && transfer.ToBranchId != _currentUserService.BranchId)
        {
            return ApiResult.Failure("Unauthorized. Only staff at the destination branch can receive this transfer.");
        }

        if (transfer.Status != TransferStatus.Shipped && transfer.Status != TransferStatus.Pending)
        {
            return ApiResult.Failure(_localizer["StockTransferStatusInvalid"]);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var detail in transfer.TransferDetails)
            {
                // 1. Update Destination BranchInventory
                var inventories = await _unitOfWork.Repository<BranchInventory>()
                    .FindAsync(i => i.BranchId == transfer.ToBranchId && i.ProductId == detail.ProductId);
                var inventory = inventories.FirstOrDefault();

                if (inventory == null)
                {
                    inventory = new BranchInventory
                    {
                        BranchId = transfer.ToBranchId,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        MinimumStockLevel = 10
                    };
                    await _unitOfWork.Repository<BranchInventory>().AddAsync(inventory);
                }
                else
                {
                    inventory.Quantity += detail.Quantity;
                    _unitOfWork.Repository<BranchInventory>().Update(inventory);
                }

                // 2. Resolve original batch details and replicate at destination
                Guid? targetBatchId = null;
                if (detail.ProductBatchId.HasValue)
                {
                    var sourceBatch = await _context.ProductBatches.FindAsync(new object[] { detail.ProductBatchId.Value }, cancellationToken);
                    if (sourceBatch != null)
                    {
                        // Check if matching batch code already exists at destination
                        var destBatches = await _unitOfWork.Repository<ProductBatch>()
                            .FindAsync(b => b.BranchId == transfer.ToBranchId && b.ProductId == detail.ProductId && b.BatchCode == sourceBatch.BatchCode);
                        var destBatch = destBatches.FirstOrDefault();

                        if (destBatch == null)
                        {
                            destBatch = new ProductBatch
                            {
                                BranchId = transfer.ToBranchId,
                                ProductId = detail.ProductId,
                                BatchCode = sourceBatch.BatchCode,
                                ExpiryDate = sourceBatch.ExpiryDate,
                                ManufacturedDate = sourceBatch.ManufacturedDate,
                                Quantity = detail.Quantity
                            };
                            await _unitOfWork.Repository<ProductBatch>().AddAsync(destBatch);
                        }
                        else
                        {
                            destBatch.Quantity += detail.Quantity;
                            _unitOfWork.Repository<ProductBatch>().Update(destBatch);
                        }

                        targetBatchId = destBatch.Id;
                    }
                }

                // 3. Create Transaction log at destination
                var transaction = new InventoryTransaction
                {
                    BranchId = transfer.ToBranchId,
                    ProductId = detail.ProductId,
                    ProductBatchId = targetBatchId,
                    Quantity = detail.Quantity, // Inflow
                    Type = TransactionType.TransferIn,
                    ReferenceCode = transfer.TransferCode,
                    Notes = $"Nhận hàng chuyển từ chi nhánh {_context.Branches.Find(transfer.FromBranchId)?.Name}"
                };
                await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
            }

            transfer.Status = TransferStatus.Received;
            _unitOfWork.Repository<StockTransfer>().Update(transfer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return ApiResult.Success(_localizer["StockTransferReceiveSuccess"]);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResult.Failure(ex.Message);
        }
    }

    public async Task<ApiResult> VerifyReplenishmentAsync(VerifyReplenishmentDto dto, CancellationToken cancellationToken = default)
    {
        var myBranchId = _currentUserService.BranchId;
        if (!myBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            if (defaultBranch == null)
            {
                return ApiResult.Failure("No default branch exists in the system.");
            }
            myBranchId = defaultBranch.Id;
        }

        Guid myBranchIdValue = myBranchId.Value;

        if (dto.Quantity <= 0)
        {
            return ApiResult.Failure(_localizer["ReplenishQuantityInvalid"]);
        }

        if (string.IsNullOrWhiteSpace(dto.LocationCode))
        {
            return ApiResult.Failure("Mã vị trí kệ không được để trống.");
        }

        var product = await _context.Products.FindAsync(new object[] { dto.ProductId }, cancellationToken);
        if (product == null)
        {
            return ApiResult.Failure(_localizer["ProductNotFound", dto.ProductId]);
        }

        // Validate stock in BranchInventory
        var inventory = await _context.BranchInventories
            .FirstOrDefaultAsync(i => i.BranchId == myBranchIdValue && i.ProductId == dto.ProductId, cancellationToken);
        
        if (inventory == null || inventory.Quantity < dto.Quantity)
        {
            var available = inventory?.Quantity ?? 0;
            return ApiResult.Failure($"Sản phẩm '{product.Name}' không đủ tồn kho chi nhánh để bồi hàng lên kệ. Tồn kho khả dụng: {available}, Yêu cầu bồi: {dto.Quantity}.");
        }

        // Resolve or create Location dynamically
        var locationCode = dto.LocationCode.Trim();
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationCode == locationCode && l.BranchId == myBranchIdValue, cancellationToken);
        if (location == null)
        {
            location = new Location
            {
                BranchId = myBranchIdValue,
                LocationCode = locationCode,
                Description = $"Kệ hàng bồi tự động {locationCode}"
            };
            await _context.Locations.AddAsync(location, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Auto-resolve batch if not specified
        Guid? finalBatchId = dto.ProductBatchId;
        if (!finalBatchId.HasValue)
        {
            var batches = await _context.ProductBatches
                .Where(b => b.BranchId == myBranchIdValue && b.ProductId == dto.ProductId && b.Quantity > 0)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync(cancellationToken);
            
            var oldestBatch = batches.FirstOrDefault();
            if (oldestBatch != null)
            {
                finalBatchId = oldestBatch.Id;
            }
        }

        // Upsert ProductLocation
        var shelfInvs = await _unitOfWork.Repository<ProductLocation>()
            .FindAsync(l => l.BranchId == myBranchIdValue && l.LocationId == location.Id && l.ProductId == dto.ProductId && l.ProductBatchId == finalBatchId);
        var shelfInv = shelfInvs.FirstOrDefault();

        if (shelfInv == null)
        {
            shelfInv = new ProductLocation
            {
                BranchId = myBranchIdValue,
                LocationId = location.Id,
                ProductId = dto.ProductId,
                ProductBatchId = finalBatchId,
                Quantity = dto.Quantity
            };
            await _unitOfWork.Repository<ProductLocation>().AddAsync(shelfInv);
        }
        else
        {
            shelfInv.Quantity += dto.Quantity;
            _unitOfWork.Repository<ProductLocation>().Update(shelfInv);
        }

        // Create transaction log
        var transaction = new InventoryTransaction
        {
            BranchId = myBranchIdValue,
            ProductId = dto.ProductId,
            ProductBatchId = finalBatchId,
            Quantity = dto.Quantity,
            Type = TransactionType.Replenish,
            ReferenceCode = $"REP-{DateTime.UtcNow:yyMMddHHmmss}",
            Notes = $"Bồi hàng lên kệ: {location.LocationCode}"
        };

        await _unitOfWork.Repository<InventoryTransaction>().AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Success(_localizer["ReplenishSuccess"]);
    }

    public async Task<ApiResult> WriteOffStockAsync(WriteOffStockDto dto, CancellationToken cancellationToken = default)
    {
        var myBranchId = _currentUserService.BranchId;
        if (!myBranchId.HasValue)
        {
            var defaultBranch = await _context.Branches.FirstOrDefaultAsync(cancellationToken);
            myBranchId = defaultBranch?.Id;
        }

        if (!myBranchId.HasValue)
        {
            return ApiResult.Failure("Không tìm thấy chi nhánh hợp lệ.");
        }

        Guid myBranchIdValue = myBranchId.Value;

        if (dto.Quantity <= 0)
        {
            return ApiResult.Failure("Số lượng hủy phải lớn hơn 0.");
        }

        var product = await _context.Products.FindAsync(new object[] { dto.ProductId }, cancellationToken);
        if (product == null)
        {
            return ApiResult.Failure("Sản phẩm không tồn tại.");
        }

        // 1. Validate and update BranchInventory
        var inventory = await _context.BranchInventories
            .FirstOrDefaultAsync(i => i.BranchId == myBranchIdValue && i.ProductId == dto.ProductId, cancellationToken);

        if (inventory == null || inventory.Quantity < dto.Quantity)
        {
            var available = inventory?.Quantity ?? 0;
            return ApiResult.Failure($"Tồn kho chi nhánh không đủ để hủy. Khả dụng: {available}, Yêu cầu hủy: {dto.Quantity}.");
        }

        inventory.Quantity -= dto.Quantity;
        _context.BranchInventories.Update(inventory);

        // 2. Resolve or decrement batch stock if specified
        Guid? batchId = null;
        if (!string.IsNullOrWhiteSpace(dto.BatchCode))
        {
            var batch = await _context.ProductBatches
                .FirstOrDefaultAsync(b => b.BranchId == myBranchIdValue && b.ProductId == dto.ProductId && b.BatchCode == dto.BatchCode, cancellationToken);
            if (batch != null)
            {
                if (batch.Quantity < dto.Quantity)
                {
                    return ApiResult.Failure($"Tồn kho của lô '{dto.BatchCode}' không đủ để hủy. Khả dụng: {batch.Quantity}.");
                }
                batch.Quantity -= dto.Quantity;
                _context.ProductBatches.Update(batch);
                batchId = batch.Id;
            }
        }
        else
        {
            // Auto-deduct from batches sequentially (FIFO)
            var batches = await _context.ProductBatches
                .Where(b => b.BranchId == myBranchIdValue && b.ProductId == dto.ProductId && b.Quantity > 0)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync(cancellationToken);

            var remainingToDeduct = dto.Quantity;
            foreach (var batch in batches)
            {
                if (remainingToDeduct <= 0) break;
                var deductAmount = Math.Min(batch.Quantity, remainingToDeduct);
                batch.Quantity -= deductAmount;
                remainingToDeduct -= deductAmount;
                _context.ProductBatches.Update(batch);
                if (!batchId.HasValue)
                {
                    batchId = batch.Id;
                }
            }
        }

        // Also decrement shelf stock (ProductLocation) if it exists
        var shelfStocks = await _context.ProductLocations
            .Where(pl => pl.BranchId == myBranchIdValue && pl.ProductId == dto.ProductId && pl.Quantity > 0)
            .OrderBy(pl => pl.Quantity)
            .ToListAsync(cancellationToken);

        var remainingToDeductShelf = dto.Quantity;
        foreach (var shelf in shelfStocks)
        {
            if (remainingToDeductShelf <= 0) break;
            var deductAmount = Math.Min(shelf.Quantity, remainingToDeductShelf);
            shelf.Quantity -= deductAmount;
            remainingToDeductShelf -= deductAmount;
            _context.ProductLocations.Update(shelf);
        }

        // 3. Create Transaction Log
        var transaction = new InventoryTransaction
        {
            BranchId = myBranchIdValue,
            ProductId = dto.ProductId,
            ProductBatchId = batchId,
            Quantity = -dto.Quantity,
            Type = TransactionType.WriteOff,
            ReferenceCode = $"WRF-{DateTime.UtcNow:yyMMddHHmmss}",
            Notes = $"Hủy hàng ({dto.Reason}): {dto.Notes}"
        };

        await _context.InventoryTransactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResult.Success("Hủy hàng thành công.");
    }

    public async Task<ApiResult<StockTransferDto>> GetStockTransferByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transfer = await _context.StockTransfers
            .Include(t => t.TransferDetails)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (transfer == null)
        {
            return ApiResult<StockTransferDto>.Failure(_localizer["StockTransferNotFound", id]);
        }

        // Branch boundary
        if (!_currentUserService.IsAdmin && transfer.FromBranchId != _currentUserService.BranchId && transfer.ToBranchId != _currentUserService.BranchId)
        {
            return ApiResult<StockTransferDto>.Failure(_localizer["StockTransferNotFound", id]);
        }

        var dto = await MapToStockTransferDtoAsync(transfer, cancellationToken);
        return ApiResult<StockTransferDto>.Success(dto);
    }

    public async Task<ApiResult<PagedResult<StockTransferDto>>> GetPagedStockTransfersAsync(
        Guid? fromBranchId, 
        Guid? toBranchId, 
        TransferStatus? status, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.StockTransfers
            .Include(t => t.TransferDetails)
            .ThenInclude(d => d.Product)
            .AsNoTracking()
            .AsQueryable();

        // Enforce boundary
        if (!_currentUserService.IsAdmin)
        {
            var myBranchId = _currentUserService.BranchId;
            query = query.Where(t => t.FromBranchId == myBranchId || t.ToBranchId == myBranchId);
        }
        else
        {
            if (fromBranchId.HasValue)
            {
                query = query.Where(t => t.FromBranchId == fromBranchId.Value);
            }
            if (toBranchId.HasValue)
            {
                query = query.Where(t => t.ToBranchId == toBranchId.Value);
            }
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = new List<StockTransferDto>();
        foreach (var item in items)
        {
            dtos.Add(await MapToStockTransferDtoAsync(item, cancellationToken));
        }

        var pagedResult = new PagedResult<StockTransferDto>(dtos, pageIndex, pageSize, totalCount);
        return ApiResult<PagedResult<StockTransferDto>>.Success(pagedResult);
    }

    private async Task<StockTransferDto> MapToStockTransferDtoAsync(StockTransfer transfer, CancellationToken cancellationToken)
    {
        var fromBranch = await _context.Branches.FindAsync(new object[] { transfer.FromBranchId }, cancellationToken);
        var toBranch = await _context.Branches.FindAsync(new object[] { transfer.ToBranchId }, cancellationToken);

        var details = new List<StockTransferDetailDto>();
        foreach (var d in transfer.TransferDetails)
        {
            string? batchCode = null;
            if (d.ProductBatchId.HasValue)
            {
                var batch = await _context.ProductBatches.FindAsync(new object[] { d.ProductBatchId.Value }, cancellationToken);
                batchCode = batch?.BatchCode;
            }

            details.Add(new StockTransferDetailDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductCode = d.Product?.Code,
                ProductName = d.Product?.Name,
                ProductBatchId = d.ProductBatchId,
                BatchCode = batchCode,
                Quantity = d.Quantity
            });
        }

        return new StockTransferDto
        {
            Id = transfer.Id,
            TransferCode = transfer.TransferCode,
            FromBranchId = transfer.FromBranchId,
            FromBranchName = fromBranch?.Name,
            ToBranchId = transfer.ToBranchId,
            ToBranchName = toBranch?.Name,
            Status = transfer.Status,
            Notes = transfer.Notes,
            CreatedAt = transfer.CreatedAt,
            TransferDetails = details
        };
    }
}
