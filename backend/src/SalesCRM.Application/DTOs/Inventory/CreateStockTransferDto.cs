namespace SalesCRM.Application.DTOs.Inventory;

public class CreateStockTransferDto
{
    public Guid ToBranchId { get; set; }
    public string? Notes { get; set; }
    public List<CreateStockTransferDetailDto> Items { get; set; } = [];
}

public class CreateStockTransferDetailDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; }
    public int Quantity { get; set; }
}
