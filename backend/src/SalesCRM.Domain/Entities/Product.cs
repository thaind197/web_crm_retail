namespace SalesCRM.Domain.Entities;

public class Product : BaseEntity
{
    public required string Code { get; set; } // Unique SKU / Product Code
    public string? Barcode { get; set; }       // Barcode for scanning
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal ImportPrice { get; set; }
    public string? ImageUrl { get; set; }      // Product Image URL/Path
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<BranchInventory> BranchInventories { get; set; } = [];
    public ICollection<ProductBatch> Batches { get; set; } = [];
    public ICollection<ProductLocation> ProductLocations { get; set; } = [];
    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}
