using MediatR;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Queries;

public record GetStockTransfersQuery(
    Guid? FromBranchId,
    Guid? ToBranchId,
    TransferStatus? Status,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<StockTransferDto>>>;

public class GetStockTransfersQueryHandler : IRequestHandler<GetStockTransfersQuery, ApiResult<PagedResult<StockTransferDto>>>
{
    private readonly IInventoryService _inventoryService;

    public GetStockTransfersQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<PagedResult<StockTransferDto>>> Handle(GetStockTransfersQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetPagedStockTransfersAsync(
            request.FromBranchId,
            request.ToBranchId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
