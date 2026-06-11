using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResult<PagedResult<UserDto>>> GetPagedUsersAsync(string? searchTerm, Guid? branchId, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResult<Guid>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<ApiResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}
