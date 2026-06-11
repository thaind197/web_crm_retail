using MediatR;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Queries;

public record GetNearExpiryProductsQuery(
    Guid? BranchId,
    int ThresholdDays = 30,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<NearExpiryProductDto>>>;

public class GetNearExpiryProductsQueryHandler : IRequestHandler<GetNearExpiryProductsQuery, ApiResult<PagedResult<NearExpiryProductDto>>>
{
    private readonly IInventoryService _inventoryService;

    public GetNearExpiryProductsQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<PagedResult<NearExpiryProductDto>>> Handle(GetNearExpiryProductsQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetNearExpiryProductsAsync(
            request.BranchId,
            request.ThresholdDays,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
