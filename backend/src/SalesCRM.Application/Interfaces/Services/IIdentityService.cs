using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<ApiResult<AuthResponse>> LoginAsync(string username, string password);
}
