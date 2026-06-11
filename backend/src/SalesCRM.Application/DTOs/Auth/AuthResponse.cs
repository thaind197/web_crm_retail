namespace SalesCRM.Application.DTOs.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public Guid? BranchId { get; set; }
    public List<string> Roles { get; set; } = [];
}
