namespace SalesCRM.Domain.Entities;

public class Branch : BaseEntity
{
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ApplicationUser> Users { get; set; } = [];
    public ICollection<BranchInventory> Inventories { get; set; } = [];
    public ICollection<ProductBatch> Batches { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
