using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Product;
using SalesCRM.Application.Features.Products.Commands;
using SalesCRM.Application.Features.Products.Queries;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ProductsController(IMediator mediator, IStringLocalizer<SharedResource> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResult<ProductDto>>>> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery(searchTerm, isActive, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<ProductDto>>> GetProductById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetProductById), new { id = result.Data }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult>> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResult.Failure(_localizer["ProductRouteIdMismatch"]));
        }

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
