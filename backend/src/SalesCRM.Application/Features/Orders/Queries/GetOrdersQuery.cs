using MediatR;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Orders.Queries;

public record GetOrdersQuery(
    string? SearchTerm,
    Guid? BranchId,
    OrderStatus? Status,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<OrderDto>>>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResult<PagedResult<OrderDto>>>
{
    private readonly IOrderService _orderService;

    public GetOrdersQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<ApiResult<PagedResult<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetPagedOrdersAsync(
            request.SearchTerm,
            request.BranchId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
