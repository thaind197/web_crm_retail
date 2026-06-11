using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesCRM.Application.DTOs.POS;
using SalesCRM.Application.Features.Orders.Commands;
using SalesCRM.Application.Features.Orders.Queries;
using SalesCRM.Domain.Enums;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<OrderDto>>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Data!.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<OrderDto>>> GetOrderById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResult<OrderDto>>>> GetOrders(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? branchId,
        [FromQuery] OrderStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrdersQuery(searchTerm, branchId, status, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id:guid}/payments")]
    public async Task<ActionResult<ApiResult<PaymentResponseDto>>> ProcessPayment(Guid id, [FromBody] ProcessPaymentCommand command)
    {
        if (id != command.OrderId)
        {
            return BadRequest(ApiResult<PaymentResponseDto>.Failure("Mã ID đơn hàng trong đường dẫn không khớp với dữ liệu gửi lên."));
        }

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
