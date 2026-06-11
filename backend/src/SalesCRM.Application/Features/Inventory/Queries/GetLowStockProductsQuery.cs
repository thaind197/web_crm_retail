using MediatR;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Queries;

public record GetLowStockProductsQuery(
    Guid? BranchId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<LowStockProductDto>>>;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, ApiResult<PagedResult<LowStockProductDto>>>
{
    private readonly IInventoryService _inventoryService;

    public GetLowStockProductsQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<PagedResult<LowStockProductDto>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetLowStockProductsAsync(
            request.BranchId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
