using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<ApiResult<Guid>> BatchImportAsync(Guid branchId, List<BatchImportItemDto> items, CancellationToken cancellationToken = default);
    Task<ApiResult<PagedResult<NearExpiryProductDto>>> GetNearExpiryProductsAsync(Guid? branchId, int thresholdDays, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<PagedResult<LowStockProductDto>>> GetLowStockProductsAsync(Guid? branchId, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<Guid>> CreateStockTransferAsync(CreateStockTransferDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult> ReceiveStockTransferAsync(Guid transferId, CancellationToken cancellationToken = default);
    Task<ApiResult> VerifyReplenishmentAsync(VerifyReplenishmentDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult> WriteOffStockAsync(WriteOffStockDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult<StockTransferDto>> GetStockTransferByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResult<PagedResult<StockTransferDto>>> GetPagedStockTransfersAsync(Guid? fromBranchId, Guid? toBranchId, TransferStatus? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
}
