using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesCRM.Application.DTOs.Reports;
using SalesCRM.Application.Features.Reports.Queries;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("branch")]
    public async Task<ActionResult<ApiResult<BranchReportDto>>> GetBranchReport(
        [FromQuery] Guid? branchId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Default to last 30 days if not specified
        if (startDate == default) startDate = DateTime.UtcNow.AddDays(-30);
        if (endDate == default) endDate = DateTime.UtcNow;

        var query = new GetBranchReportQuery(branchId, startDate, endDate);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("system")]
    public async Task<ActionResult<ApiResult<SystemReportDto>>> GetSystemReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default) startDate = DateTime.UtcNow.AddDays(-30);
        if (endDate == default) endDate = DateTime.UtcNow;

        var query = new GetSystemReportQuery(startDate, endDate);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
