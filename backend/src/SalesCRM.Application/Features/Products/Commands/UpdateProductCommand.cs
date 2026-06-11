using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Code,
    string? Barcode,
    string Name,
    string? Description,
    decimal SellingPrice,
    decimal ImportPrice,
    string? ImageUrl,
    string? ImageBase64,
    bool IsActive
) : IRequest<ApiResult>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResult>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ApiResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.UpdateProductAsync(
            request.Id,
            request.Code,
            request.Barcode,
            request.Name,
            request.Description,
            request.SellingPrice,
            request.ImportPrice,
            request.ImageUrl,
            request.ImageBase64,
            request.IsActive,
            cancellationToken);
    }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["ProductIdRequired"]);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(localizer["ProductCodeRequired"])
            .MaximumLength(50).WithMessage(localizer["ProductCodeMaxLength"]);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(localizer["ProductNameRequired"])
            .MaximumLength(200).WithMessage(localizer["ProductNameMaxLength"]);

        RuleFor(x => x.SellingPrice)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["ProductSellingPriceRange"]);

        RuleFor(x => x.ImportPrice)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["ProductImportPriceRange"]);
    }
}
