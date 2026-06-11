namespace SalesCRM.Domain.Entities;

public class ProductBatch : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid ProductId { get; set; }
    public required string BatchCode { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? ManufacturedDate { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public Product? Product { get; set; }
}
