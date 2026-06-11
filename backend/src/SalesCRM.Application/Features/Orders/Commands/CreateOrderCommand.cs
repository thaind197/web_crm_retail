using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Orders.Commands;

public record CreateOrderCommand(
    Guid? CustomerId,
    decimal Discount,
    List<CreateOrderDetailCommandDto> Items,
    PaymentMethod? PaymentMethod = null,
    decimal? PaymentAmount = null,
    string? CouponCode = null
) : IRequest<ApiResult<OrderDto>>;

public class CreateOrderDetailCommandDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<OrderDto>>
{
    private readonly IOrderService _orderService;

    public CreateOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<ApiResult<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateOrderDto
        {
            CustomerId = request.CustomerId,
            Discount = request.Discount,
            CouponCode = request.CouponCode,
            PaymentMethod = request.PaymentMethod,
            PaymentAmount = request.PaymentAmount,
            Items = request.Items.Select(i => new CreateOrderDetailDto
            {
                ProductId = i.ProductId,
                ProductBatchId = i.ProductBatchId,
                Quantity = i.Quantity
            }).ToList()
        };

        return await _orderService.CreateOrderAsync(dto, cancellationToken);
    }
}

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(localizer["ItemsCannotBeEmpty"] ?? "Giỏ hàng không được để trống.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage(localizer["ProductIdRequired"]);

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage(localizer["ProductMinStockLevel"] ?? "Số lượng bán phải lớn hơn 0.");
        });

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["ProductSellingPriceRange"] ?? "Chiết khấu không được âm.");

        RuleFor(x => x.PaymentAmount)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["PaymentAmountInvalid"] ?? "Số tiền thanh toán không được âm.")
            .When(x => x.PaymentAmount.HasValue);
    }
}
