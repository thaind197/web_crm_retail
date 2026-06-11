using MediatR;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Commands;

public record ReceiveStockTransferCommand(Guid TransferId) : IRequest<ApiResult>;

public class ReceiveStockTransferCommandHandler : IRequestHandler<ReceiveStockTransferCommand, ApiResult>
{
    private readonly IInventoryService _inventoryService;

    public ReceiveStockTransferCommandHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult> Handle(ReceiveStockTransferCommand request, CancellationToken cancellationToken)
    {
        return await _inventoryService.ReceiveStockTransferAsync(request.TransferId, cancellationToken);
    }
}
