namespace SalesCRM.Application.DTOs.Inventory;

public class BatchImportItemDto
{
    public Guid ProductId { get; set; }
    public required string BatchCode { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime? ManufacturedDate { get; set; }
    public int Quantity { get; set; }
    public int MinimumStockLevel { get; set; } // Set minimum warning level
}
