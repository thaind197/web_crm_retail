using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SalesCRM.Application.Interfaces.Services;

namespace SalesCRM.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public Guid? BranchId
    {
        get
        {
            var branchIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue("BranchId");
            if (Guid.TryParse(branchIdStr, out var branchId))
            {
                return branchId;
            }
            return null;
        }
    }

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public bool IsAdmin => Role == "Admin" || Role == "SuperAdmin";
}
