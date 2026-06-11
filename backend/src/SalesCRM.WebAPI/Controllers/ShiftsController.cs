using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;
using System;
using System.Threading.Tasks;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ShiftsController(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<ApiResult<ShiftDto?>>> GetCurrentShift()
    {
        if (string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return BadRequest(ApiResult<ShiftDto?>.Failure("User ID is not resolved."));
        }

        var userId = Guid.Parse(_currentUserService.UserId);
        var branchId = _currentUserService.BranchId;

        if (!branchId.HasValue)
        {
            var defaultBranch = await _dbContext.Branches.FirstOrDefaultAsync();
            if (defaultBranch != null)
            {
                branchId = defaultBranch.Id;
            }
        }

        if (!branchId.HasValue)
        {
            return BadRequest(ApiResult<ShiftDto?>.Failure("Branch ID is not resolved."));
        }

        var shift = await _dbContext.Shifts
            .FirstOrDefaultAsync(s => s.UserId == userId && s.BranchId == branchId.Value && s.Status == "Open");

        if (shift == null)
        {
            return Ok(ApiResult<ShiftDto?>.Success(null));
        }

        var dto = new ShiftDto
        {
            Id = shift.Id,
            BranchId = shift.BranchId,
            UserId = shift.UserId,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            StartCash = shift.StartCash,
            EndCashCounted = shift.EndCashCounted,
            TotalSalesCash = shift.TotalSalesCash,
            TotalSalesMomo = shift.TotalSalesMomo,
            TotalSalesVNPay = shift.TotalSalesVNPay,
            TotalSalesBank = shift.TotalSalesBank,
            Status = shift.Status,
            Notes = shift.Notes
        };

        return Ok(ApiResult<ShiftDto>.Success(dto));
    }

    [HttpPost("open")]
    public async Task<ActionResult<ApiResult<ShiftDto>>> OpenShift([FromBody] OpenShiftRequest model)
    {
        if (string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("User ID is not resolved."));
        }

        var userId = Guid.Parse(_currentUserService.UserId);
        var branchId = _currentUserService.BranchId;

        if (!branchId.HasValue)
        {
            var defaultBranch = await _dbContext.Branches.FirstOrDefaultAsync();
            if (defaultBranch != null)
            {
                branchId = defaultBranch.Id;
            }
        }

        if (!branchId.HasValue)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Branch ID is not resolved."));
        }

        if (model.StartCash < 0)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Số tiền đầu ca không được âm."));
        }

        // Check if there is already an active shift
        var existingShift = await _dbContext.Shifts
            .AnyAsync(s => s.UserId == userId && s.BranchId == branchId.Value && s.Status == "Open");

        if (existingShift)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Bạn đang có một ca làm việc chưa kết thúc."));
        }

        var shift = new Shift
        {
            BranchId = branchId.Value,
            UserId = userId,
            StartTime = DateTime.UtcNow,
            StartCash = model.StartCash,
            Status = "Open",
            TotalSalesCash = 0,
            TotalSalesMomo = 0,
            TotalSalesVNPay = 0,
            TotalSalesBank = 0
        };

        _dbContext.Shifts.Add(shift);
        await _dbContext.SaveChangesAsync();

        var dto = new ShiftDto
        {
            Id = shift.Id,
            BranchId = shift.BranchId,
            UserId = shift.UserId,
            StartTime = shift.StartTime,
            StartCash = shift.StartCash,
            Status = shift.Status,
            TotalSalesCash = 0,
            TotalSalesMomo = 0,
            TotalSalesVNPay = 0,
            TotalSalesBank = 0
        };

        return Ok(ApiResult<ShiftDto>.Success(dto, "Mở ca làm việc thành công."));
    }

    [HttpPost("close")]
    public async Task<ActionResult<ApiResult<ShiftDto>>> CloseShift([FromBody] CloseShiftRequest model)
    {
        if (string.IsNullOrEmpty(_currentUserService.UserId))
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("User ID is not resolved."));
        }

        var userId = Guid.Parse(_currentUserService.UserId);
        var branchId = _currentUserService.BranchId;

        if (!branchId.HasValue)
        {
            var defaultBranch = await _dbContext.Branches.FirstOrDefaultAsync();
            if (defaultBranch != null)
            {
                branchId = defaultBranch.Id;
            }
        }

        if (!branchId.HasValue)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Branch ID is not resolved."));
        }

        if (model.EndCashCounted < 0)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Số tiền mặt kiểm kê thực tế không được âm."));
        }

        var shift = await _dbContext.Shifts
            .FirstOrDefaultAsync(s => s.UserId == userId && s.BranchId == branchId.Value && s.Status == "Open");

        if (shift == null)
        {
            return BadRequest(ApiResult<ShiftDto>.Failure("Không tìm thấy ca làm việc đang hoạt động của bạn."));
        }

        shift.EndTime = DateTime.UtcNow;
        shift.EndCashCounted = model.EndCashCounted;
        shift.Status = "Closed";
        shift.Notes = model.Notes;

        _dbContext.Shifts.Update(shift);
        await _dbContext.SaveChangesAsync();

        var dto = new ShiftDto
        {
            Id = shift.Id,
            BranchId = shift.BranchId,
            UserId = shift.UserId,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            StartCash = shift.StartCash,
            EndCashCounted = shift.EndCashCounted,
            TotalSalesCash = shift.TotalSalesCash,
            TotalSalesMomo = shift.TotalSalesMomo,
            TotalSalesVNPay = shift.TotalSalesVNPay,
            TotalSalesBank = shift.TotalSalesBank,
            Status = shift.Status,
            Notes = shift.Notes
        };

        return Ok(ApiResult<ShiftDto>.Success(dto, "Kết ca làm việc thành công."));
    }
}

public class ShiftDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal StartCash { get; set; }
    public decimal? EndCashCounted { get; set; }
    public decimal TotalSalesCash { get; set; }
    public decimal TotalSalesMomo { get; set; }
    public decimal TotalSalesVNPay { get; set; }
    public decimal TotalSalesBank { get; set; }
    public string Status { get; set; } = "Open";
    public string? Notes { get; set; }
}

public class OpenShiftRequest
{
    public decimal StartCash { get; set; }
}

public class CloseShiftRequest
{
    public decimal EndCashCounted { get; set; }
    public string? Notes { get; set; }
}
