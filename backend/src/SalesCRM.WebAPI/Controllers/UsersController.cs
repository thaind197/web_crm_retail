using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Features.Users.Commands;
using SalesCRM.Application.Features.Users.Queries;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize(Roles = "Admin,Manager")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public UsersController(IMediator mediator, IStringLocalizer<SharedResource> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? branchId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(searchTerm, branchId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetUserById), new { id = result.Data }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult>> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(ApiResult.Failure(_localizer["RouteIdMismatch"]));
        }

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
