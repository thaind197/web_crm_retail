namespace SalesCRM.Domain.Entities;

public class ProductLocation : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; } // Tùy chọn liên kết với lô cụ thể
    public int Quantity { get; set; }

    // Navigation properties
    public Location? Location { get; set; }
    public Product? Product { get; set; }
    public ProductBatch? ProductBatch { get; set; }
}
