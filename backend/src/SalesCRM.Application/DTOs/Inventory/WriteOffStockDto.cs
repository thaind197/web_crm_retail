namespace SalesCRM.Application.DTOs.Inventory;

public class WriteOffStockDto
{
    public Guid ProductId { get; set; }
    public string? BatchCode { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
