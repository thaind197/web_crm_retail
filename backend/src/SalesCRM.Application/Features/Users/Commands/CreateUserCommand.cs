using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Password,
    string FullName,
    string Email,
    Guid? BranchId,
    string Role
) : IRequest<ApiResult<Guid>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResult<Guid>>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new CreateUserDto
        {
            Username = request.Username,
            Password = request.Password,
            FullName = request.FullName,
            Email = request.Email,
            BranchId = request.BranchId,
            Role = request.Role
        };

        return await _userService.CreateUserAsync(dto, cancellationToken);
    }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(localizer["UserUsernameRequired"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["UserPasswordRequired"])
            .MinimumLength(6).WithMessage(localizer["UserPasswordRequired"]);

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(localizer["UserFullNameRequired"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["UserEmailRequired"])
            .EmailAddress().WithMessage(localizer["UserEmailInvalid"]);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer["UserRoleRequired"]);
    }
}
