using MediatR;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<ApiResult<AuthResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult<AuthResponse>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ApiResult<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Username, request.Password);
    }
}
