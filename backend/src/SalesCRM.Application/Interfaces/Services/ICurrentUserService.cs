namespace SalesCRM.Application.Interfaces.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid? BranchId { get; }
    string? Username { get; }
    string? Role { get; }
    bool IsAdmin { get; }
}
