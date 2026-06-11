using SalesCRM.Domain.Enums;

namespace SalesCRM.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; } // Số lượng thay đổi (dương là tăng, âm là giảm)
    public TransactionType Type { get; set; }
    public string? ReferenceCode { get; set; } // Mã chứng từ tham chiếu (Mã đơn hàng, phiếu nhập, phiếu chuyển)
    public string? Notes { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public Product? Product { get; set; }
    public ProductBatch? ProductBatch { get; set; }
}
