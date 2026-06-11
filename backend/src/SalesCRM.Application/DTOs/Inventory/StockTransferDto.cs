using SalesCRM.Domain.Enums;

namespace SalesCRM.Application.DTOs.Inventory;

public class StockTransferDto
{
    public Guid Id { get; set; }
    public required string TransferCode { get; set; }
    public Guid FromBranchId { get; set; }
    public string? FromBranchName { get; set; }
    public Guid ToBranchId { get; set; }
    public string? ToBranchName { get; set; }
    public TransferStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<StockTransferDetailDto> TransferDetails { get; set; } = [];
}

public class StockTransferDetailDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public Guid? ProductBatchId { get; set; }
    public string? BatchCode { get; set; }
    public int Quantity { get; set; }
}
