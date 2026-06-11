namespace SalesCRM.Application.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
