namespace SalesCRM.Application.DTOs.Inventory;

public class NearExpiryProductDto
{
    public Guid ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public Guid BatchId { get; set; }
    public required string BatchCode { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
    public int Quantity { get; set; }
}
