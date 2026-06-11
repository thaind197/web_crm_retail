using SalesCRM.Domain.Enums;

namespace SalesCRM.Domain.Entities;

public class StockTransfer : BaseEntity
{
    public required string TransferCode { get; set; }
    public Guid FromBranchId { get; set; }
    public Guid ToBranchId { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public string? Notes { get; set; }

    // Navigation properties
    public Branch? FromBranch { get; set; }
    public Branch? ToBranch { get; set; }
    public ICollection<StockTransferDetail> TransferDetails { get; set; } = [];
}
