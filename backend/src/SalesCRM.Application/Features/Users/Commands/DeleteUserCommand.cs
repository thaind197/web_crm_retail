using MediatR;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<ApiResult>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResult>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.DeleteUserAsync(request.Id, cancellationToken);
    }
}
