using SalesCRM.Application.DTOs.POS;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IOrderService
{
    Task<ApiResult<OrderDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult<OrderDto>> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResult<PagedResult<OrderDto>>> GetPagedOrdersAsync(string? searchTerm, Guid? branchId, OrderStatus? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<PaymentResponseDto>> ProcessOrderPaymentAsync(Guid orderId, PaymentMethod method, decimal amount, CancellationToken cancellationToken = default);
}
