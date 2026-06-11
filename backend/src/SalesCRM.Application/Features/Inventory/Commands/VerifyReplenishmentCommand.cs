using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Commands;

public record VerifyReplenishmentCommand(
    string LocationCode,
    Guid ProductId,
    Guid? ProductBatchId,
    int Quantity
) : IRequest<ApiResult>;

public class VerifyReplenishmentCommandHandler : IRequestHandler<VerifyReplenishmentCommand, ApiResult>
{
    private readonly IInventoryService _inventoryService;

    public VerifyReplenishmentCommandHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult> Handle(VerifyReplenishmentCommand request, CancellationToken cancellationToken)
    {
        var dto = new VerifyReplenishmentDto
        {
            LocationCode = request.LocationCode,
            ProductId = request.ProductId,
            ProductBatchId = request.ProductBatchId,
            Quantity = request.Quantity
        };

        return await _inventoryService.VerifyReplenishmentAsync(dto, cancellationToken);
    }
}

public class VerifyReplenishmentCommandValidator : AbstractValidator<VerifyReplenishmentCommand>
{
    public VerifyReplenishmentCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.LocationCode)
            .NotEmpty().WithMessage(localizer["LocationCodeRequired"] ?? "Mã vị trí kệ không được để trống.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(localizer["ProductIdRequired"]);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(localizer["ReplenishQuantityInvalid"]);
    }
}
