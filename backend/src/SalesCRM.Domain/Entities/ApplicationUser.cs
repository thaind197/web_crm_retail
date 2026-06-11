using Microsoft.AspNetCore.Identity;

namespace SalesCRM.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public Guid? BranchId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Branch? Branch { get; set; }
}

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
