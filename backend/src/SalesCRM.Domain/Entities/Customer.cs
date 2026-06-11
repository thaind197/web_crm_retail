namespace SalesCRM.Domain.Entities;

public class Customer : BaseEntity
{
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }

    // Navigation property
    public ICollection<Order> Orders { get; set; } = [];
}
