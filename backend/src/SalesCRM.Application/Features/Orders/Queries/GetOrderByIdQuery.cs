using MediatR;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<ApiResult<OrderDto>>;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ApiResult<OrderDto>>
{
    private readonly IOrderService _orderService;

    public GetOrderByIdQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<ApiResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetOrderByIdAsync(request.Id, cancellationToken);
    }
}
