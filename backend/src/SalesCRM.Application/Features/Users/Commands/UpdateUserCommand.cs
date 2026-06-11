using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Auth;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string FullName,
    string Email,
    Guid? BranchId,
    string Role,
    bool IsActive,
    string? Password = null
) : IRequest<ApiResult>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResult>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new UpdateUserDto
        {
            FullName = request.FullName,
            Email = request.Email,
            BranchId = request.BranchId,
            Role = request.Role,
            IsActive = request.IsActive,
            Password = request.Password
        };

        return await _userService.UpdateUserAsync(request.Id, dto, cancellationToken);
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(localizer["UserFullNameRequired"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["UserEmailRequired"])
            .EmailAddress().WithMessage(localizer["UserEmailInvalid"]);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage(localizer["UserRoleRequired"]);

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage(localizer["UserPasswordRequired"])
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}
