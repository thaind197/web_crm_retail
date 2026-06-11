namespace SalesCRM.Domain.Entities;

public class StockTransferDetail : BaseEntity
{
    public Guid StockTransferId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public StockTransfer? StockTransfer { get; set; }
    public Product? Product { get; set; }
    public ProductBatch? ProductBatch { get; set; }
}
