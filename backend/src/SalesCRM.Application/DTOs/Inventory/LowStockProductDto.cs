namespace SalesCRM.Application.DTOs.Inventory;

public class LowStockProductDto
{
    public Guid ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public int CurrentQuantity { get; set; }
    public int MinimumStockLevel { get; set; }
}
