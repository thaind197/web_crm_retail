using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Commands;

public record CreateStockTransferCommand(
    Guid ToBranchId,
    string? Notes,
    List<CreateStockTransferDetailCommandDto> Items
) : IRequest<ApiResult<Guid>>;

public class CreateStockTransferDetailCommandDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; }
}

public class CreateStockTransferCommandHandler : IRequestHandler<CreateStockTransferCommand, ApiResult<Guid>>
{
    private readonly IInventoryService _inventoryService;

    public CreateStockTransferCommandHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<Guid>> Handle(CreateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateStockTransferDto
        {
            ToBranchId = request.ToBranchId,
            Notes = request.Notes,
            Items = request.Items.Select(i => new CreateStockTransferDetailDto
            {
                ProductId = i.ProductId,
                ProductBatchId = i.ProductBatchId,
                Quantity = i.Quantity
            }).ToList()
        };

        return await _inventoryService.CreateStockTransferAsync(dto, cancellationToken);
    }
}

public class CreateStockTransferCommandValidator : AbstractValidator<CreateStockTransferCommand>
{
    public CreateStockTransferCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.ToBranchId)
            .NotEmpty().WithMessage(localizer["BranchIdRequired"] ?? "ID chi nhánh nhận không được để trống.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(localizer["StockTransferDetailNotFound"] ?? "Danh sách chuyển kho không được để trống.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage(localizer["ProductIdRequired"]);

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage(localizer["ProductMinStockLevel"] ?? "Số lượng chuyển phải lớn hơn 0.");
        });
    }
}
