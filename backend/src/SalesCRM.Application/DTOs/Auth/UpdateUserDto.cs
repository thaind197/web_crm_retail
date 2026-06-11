namespace SalesCRM.Application.DTOs.Auth;

public class UpdateUserDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public Guid? BranchId { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; }
    public string? Password { get; set; } // Optional password update
}
