namespace SalesCRM.Application.DTOs.Auth;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public Guid? BranchId { get; set; }
    public required string Role { get; set; }
}
