using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Commands;

public record WriteOffStockCommand(
    Guid ProductId,
    string? BatchCode,
    int Quantity,
    string Reason,
    string? Notes
) : IRequest<ApiResult>;

public class WriteOffStockCommandHandler : IRequestHandler<WriteOffStockCommand, ApiResult>
{
    private readonly IInventoryService _inventoryService;

    public WriteOffStockCommandHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult> Handle(WriteOffStockCommand request, CancellationToken cancellationToken)
    {
        var dto = new WriteOffStockDto
        {
            ProductId = request.ProductId,
            BatchCode = request.BatchCode,
            Quantity = request.Quantity,
            Reason = request.Reason,
            Notes = request.Notes
        };

        return await _inventoryService.WriteOffStockAsync(dto, cancellationToken);
    }
}

public class WriteOffStockCommandValidator : AbstractValidator<WriteOffStockCommand>
{
    public WriteOffStockCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(localizer["ProductIdRequired"] ?? "Mã sản phẩm không được để trống.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Số lượng hủy phải lớn hơn 0.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do hủy không được để trống.");
    }
}
