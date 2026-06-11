using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Inventory;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Inventory.Commands;

public record BatchImportCommand(
    Guid BranchId,
    List<BatchImportItemCommandDto> Items
) : IRequest<ApiResult<Guid>>;

public class BatchImportItemCommandDto
{
    public Guid ProductId { get; set; }
    public required string BatchCode { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? ManufacturedDate { get; set; }
    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; }
}

public class BatchImportCommandHandler : IRequestHandler<BatchImportCommand, ApiResult<Guid>>
{
    private readonly IInventoryService _inventoryService;

    public BatchImportCommandHandler(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<ApiResult<Guid>> Handle(BatchImportCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => new BatchImportItemDto
        {
            ProductId = i.ProductId,
            BatchCode = i.BatchCode,
            ExpiryDate = i.ExpiryDate,
            ManufacturedDate = i.ManufacturedDate,
            Quantity = i.Quantity,
            MinimumStockLevel = i.MinimumStockLevel
        }).ToList();

        return await _inventoryService.BatchImportAsync(request.BranchId, items, cancellationToken);
    }
}

public class BatchImportCommandValidator : AbstractValidator<BatchImportCommand>
{
    public BatchImportCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage(localizer["BranchIdRequired"] ?? "ID chi nhánh không được để trống.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(localizer["ItemsCannotBeEmpty"] ?? "Danh sách nhập hàng không được để trống.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage(localizer["ProductIdRequired"]);

            item.RuleFor(i => i.BatchCode)
                .NotEmpty().WithMessage(localizer["ProductCodeRequired"] ?? "Mã lô hàng không được trống.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage(localizer["ProductMinStockLevel"] ?? "Số lượng nhập phải lớn hơn 0.");

            item.RuleFor(i => i.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage(localizer["ExpiryDateInvalid"] ?? "Hạn sử dụng phải lớn hơn ngày hiện tại.");
        });
    }
}
