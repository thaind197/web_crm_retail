using MediatR;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Users.Queries;

public record GetUsersQuery(
    string? SearchTerm,
    Guid? BranchId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<UserDto>>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ApiResult<PagedResult<UserDto>>>
{
    private readonly IUserService _userService;

    public GetUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetPagedUsersAsync(
            request.SearchTerm,
            request.BranchId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
