using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Entities;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IStringLocalizer<SharedResource> localizer)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _localizer = localizer;
    }

    public async Task<ApiResult<AuthResponse>> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return ApiResult<AuthResponse>.Failure(_localizer["LoginInvalidCredentials"]);
        }

        if (!user.IsActive)
        {
            return ApiResult<AuthResponse>.Failure(_localizer["LoginAccountLocked"]);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return ApiResult<AuthResponse>.Failure(_localizer["LoginInvalidCredentials"]);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);

        var response = new AuthResponse
        {
            Token = token,
            Username = user.UserName ?? string.Empty,
            FullName = user.FullName,
            BranchId = user.BranchId,
            Roles = roles.ToList()
        };

        return ApiResult<AuthResponse>.Success(response, _localizer["LoginSuccess"]);
    }
}
