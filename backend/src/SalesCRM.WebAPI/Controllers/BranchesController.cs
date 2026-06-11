using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public BranchesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<List<Branch>>>> GetBranches()
    {
        var branches = await _dbContext.Branches
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync();
        return Ok(ApiResult<List<Branch>>.Success(branches));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> CreateBranch([FromBody] Branch model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return BadRequest(ApiResult<Guid>.Failure("Tên chi nhánh không được để trống."));
        }

        var branch = new Branch
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                IsActive = model.IsActive
            };

        _dbContext.Branches.Add(branch);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBranches), new { }, ApiResult<Guid>.Success(branch.Id));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult>> UpdateBranch(Guid id, [FromBody] Branch model)
    {
        var branch = await _dbContext.Branches.FindAsync(id);
        if (branch == null)
        {
            return NotFound(ApiResult.Failure("Không tìm thấy chi nhánh."));
        }

        branch.Name = model.Name;
        branch.Address = model.Address;
        branch.Phone = model.Phone;
        branch.IsActive = model.IsActive;

        await _dbContext.SaveChangesAsync();
        return Ok(ApiResult.Success());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteBranch(Guid id)
    {
        var branch = await _dbContext.Branches.FindAsync(id);
        if (branch == null)
        {
            return NotFound(ApiResult.Failure("Không tìm thấy chi nhánh."));
        }

        // Soft delete or hard delete depending on design. Let's do simple hard delete, but handle exceptions if data references exist
        try
        {
            _dbContext.Branches.Remove(branch);
            await _dbContext.SaveChangesAsync();
            return Ok(ApiResult.Success());
        }
        catch (Exception)
        {
            return BadRequest(ApiResult.Failure("Không thể xóa chi nhánh do đã có dữ liệu giao dịch liên quan. Hãy chuyển trạng thái hoạt động sang Vô hiệu hóa thay thế."));
        }
    }
}
