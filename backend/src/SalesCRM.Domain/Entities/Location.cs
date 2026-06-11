namespace SalesCRM.Domain.Entities;

public class Location : BaseEntity
{
    public Guid BranchId { get; set; }
    public required string LocationCode { get; set; } // Mã kệ (ví dụ: KE-A1-02)
    public string? Description { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public ICollection<ProductLocation> ProductLocations { get; set; } = [];
}
