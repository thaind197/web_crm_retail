using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesCRM.Application.Features.Auth.Commands;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<AuthResponse>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
