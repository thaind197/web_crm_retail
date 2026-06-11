namespace SalesCRM.Application.DTOs.Inventory;

public class VerifyReplenishmentDto
{
    public string LocationCode { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; }
}
