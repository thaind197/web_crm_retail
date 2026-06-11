namespace SalesCRM.Domain.Entities;

public class BranchInventory : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; } // Ngưỡng tối thiểu để cảnh báo thiếu hàng

    // Navigation properties
    public Branch? Branch { get; set; }
    public Product? Product { get; set; }
}
