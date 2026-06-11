using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Products.Commands;

public record CreateProductCommand(
    string? Code,
    string? Barcode,
    string Name,
    string? Description,
    decimal SellingPrice,
    decimal ImportPrice,
    string? ImageUrl,
    string? ImageBase64,
    bool IsActive = true
) : IRequest<ApiResult<Guid>>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResult<Guid>>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ApiResult<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.CreateProductAsync(
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

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Code)
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
