using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Repositories;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IStringLocalizer<SharedResource> localizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }

    public async Task<ApiResult<PagedResult<UserDto>>> GetPagedUsersAsync(
        string? searchTerm, 
        Guid? branchId, 
        int pageIndex, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.AsQueryable();

        // Enforce branch boundary
        if (!_currentUserService.IsAdmin)
        {
            var myBranchId = _currentUserService.BranchId;
            query = query.Where(u => u.BranchId == myBranchId);
        }
        else if (branchId.HasValue)
        {
            query = query.Where(u => u.BranchId == branchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(u => (u.UserName != null && u.UserName.ToLower().Contains(term)) || 
                                 u.FullName.ToLower().Contains(term) || 
                                 (u.Email != null && u.Email.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtoItems = new List<UserDto>();
        foreach (var user in items)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleName = userRoles.FirstOrDefault() ?? "Staff";

            string? branchName = null;
            if (user.BranchId.HasValue)
            {
                var branch = await _context.Branches.FindAsync(new object[] { user.BranchId.Value }, cancellationToken);
                branchName = branch?.Name;
            }

            dtoItems.Add(new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                BranchId = user.BranchId,
                BranchName = branchName,
                Role = roleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        var pagedResult = new PagedResult<UserDto>(dtoItems, pageIndex, pageSize, totalCount);
        return ApiResult<PagedResult<UserDto>>.Success(pagedResult);
    }

    public async Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ApiResult<UserDto>.Failure(_localizer["UserNotFound", id]);
        }

        // Enforce branch boundary
        if (!_currentUserService.IsAdmin && user.BranchId != _currentUserService.BranchId)
        {
            return ApiResult<UserDto>.Failure(_localizer["UserNotFound", id]);
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var roleName = userRoles.FirstOrDefault() ?? "Staff";

        string? branchName = null;
        if (user.BranchId.HasValue)
        {
            var branch = await _context.Branches.FindAsync(new object[] { user.BranchId.Value }, cancellationToken);
            branchName = branch?.Name;
        }

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            BranchId = user.BranchId,
            BranchName = branchName,
            Role = roleName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return ApiResult<UserDto>.Success(dto);
    }

    public async Task<ApiResult<Guid>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // Enforce branch boundary
        Guid? targetBranchId = _currentUserService.IsAdmin ? dto.BranchId : _currentUserService.BranchId;

        // Check if username already exists
        if (await _userManager.FindByNameAsync(dto.Username) != null)
        {
            return ApiResult<Guid>.Failure(_localizer["UsernameExists", dto.Username]);
        }

        // Check if email already exists
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
        {
            return ApiResult<Guid>.Failure(_localizer["EmailExists", dto.Email]);
        }

        // Verify Branch
        if (targetBranchId.HasValue)
        {
            var branch = await _context.Branches.FindAsync(new object[] { targetBranchId.Value }, cancellationToken);
            if (branch == null)
            {
                return ApiResult<Guid>.Failure(_localizer["BranchNotFound", targetBranchId.Value]);
            }
        }

        // Verify Role
        if (!await _roleManager.RoleExistsAsync(dto.Role))
        {
            return ApiResult<Guid>.Failure(_localizer["RoleNotFound", dto.Role]);
        }

        var user = new ApplicationUser
        {
            UserName = dto.Username.Trim(),
            Email = dto.Email.Trim(),
            FullName = dto.FullName.Trim(),
            BranchId = targetBranchId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Error creating user.";
            return ApiResult<Guid>.Failure(firstError);
        }

        await _userManager.AddToRoleAsync(user, dto.Role);
        return ApiResult<Guid>.Success(user.Id, _localizer["UserCreateSuccess"]);
    }

    public async Task<ApiResult> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ApiResult.Failure(_localizer["UserNotFound", id]);
        }

        // Enforce branch boundary
        if (!_currentUserService.IsAdmin && user.BranchId != _currentUserService.BranchId)
        {
            return ApiResult.Failure(_localizer["UserNotFound", id]);
        }

        Guid? targetBranchId = _currentUserService.IsAdmin ? dto.BranchId : _currentUserService.BranchId;

        // Check email conflict
        var emailUser = await _userManager.FindByEmailAsync(dto.Email);
        if (emailUser != null && emailUser.Id != id)
        {
            return ApiResult.Failure(_localizer["EmailExists", dto.Email]);
        }

        // Verify Branch
        if (targetBranchId.HasValue)
        {
            var branch = await _context.Branches.FindAsync(new object[] { targetBranchId.Value }, cancellationToken);
            if (branch == null)
            {
                return ApiResult.Failure(_localizer["BranchNotFound", targetBranchId.Value]);
            }
        }

        // Verify Role
        if (!await _roleManager.RoleExistsAsync(dto.Role))
        {
            return ApiResult.Failure(_localizer["RoleNotFound", dto.Role]);
        }

        user.FullName = dto.FullName.Trim();
        user.Email = dto.Email.Trim();
        user.BranchId = targetBranchId;
        user.IsActive = dto.IsActive;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Error updating user.";
            return ApiResult.Failure(firstError);
        }

        // Update Role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);

        // Update Password if provided
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!resetResult.Succeeded)
            {
                var firstError = resetResult.Errors.FirstOrDefault()?.Description ?? "Error updating password.";
                return ApiResult.Failure(firstError);
            }
        }

        return ApiResult.Success(_localizer["UserUpdateSuccess"]);
    }

    public async Task<ApiResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return ApiResult.Failure(_localizer["UserNotFound", id]);
        }

        // Enforce branch boundary
        if (!_currentUserService.IsAdmin && user.BranchId != _currentUserService.BranchId)
        {
            return ApiResult.Failure(_localizer["UserNotFound", id]);
        }

        // Cannot delete self
        var currentUserIdStr = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(currentUserIdStr) && Guid.TryParse(currentUserIdStr, out var currentUserId) && currentUserId == id)
        {
            return ApiResult.Failure(_localizer["UserDeleteAdminSelfError"]);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Error deleting user.";
            return ApiResult.Failure(firstError);
        }

        return ApiResult.Success(_localizer["UserDeleteSuccess"]);
    }
}
