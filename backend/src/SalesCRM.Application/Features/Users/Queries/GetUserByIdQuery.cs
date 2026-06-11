using MediatR;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<ApiResult<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResult<UserDto>>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByIdAsync(request.Id, cancellationToken);
    }
}
