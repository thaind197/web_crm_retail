using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Orders.Commands;

public record ProcessPaymentCommand(
    Guid OrderId,
    PaymentMethod PaymentMethod,
    decimal Amount
) : IRequest<ApiResult<PaymentResponseDto>>;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ApiResult<PaymentResponseDto>>
{
    private readonly IOrderService _orderService;

    public ProcessPaymentCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<ApiResult<PaymentResponseDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.ProcessOrderPaymentAsync(
            request.OrderId,
            request.PaymentMethod,
            request.Amount,
            cancellationToken);
    }
}

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(localizer["OrderIdRequired"] ?? "ID đơn hàng bắt buộc phải có.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(localizer["PaymentAmountInvalid"]);
    }
}
