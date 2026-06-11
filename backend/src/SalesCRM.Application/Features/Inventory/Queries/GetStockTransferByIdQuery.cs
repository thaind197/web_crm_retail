using MediatR;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Queries;

public record GetStockTransferByIdQuery(Guid Id) : IRequest<ApiResult<StockTransferDto>>;

public class GetStockTransferByIdQueryHandler : IRequestHandler<GetStockTransferByIdQuery, ApiResult<StockTransferDto>>
{
    private readonly IInventoryService _inventoryService;

    public GetStockTransferByIdQueryHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<StockTransferDto>> Handle(GetStockTransferByIdQuery request, CancellationToken cancellationToken)
    {
        return await _inventoryService.GetStockTransferByIdAsync(request.Id, cancellationToken);
    }
}
