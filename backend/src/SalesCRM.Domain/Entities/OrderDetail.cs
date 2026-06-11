namespace SalesCRM.Domain.Entities;

public class OrderDetail : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; } // Liên kết nếu trừ kho theo lô cụ thể
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => Quantity * UnitPrice;

    // Navigation properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public ProductBatch? ProductBatch { get; set; }
}
