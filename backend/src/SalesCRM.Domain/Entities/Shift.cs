using System;

namespace SalesCRM.Domain.Entities;

public class Shift : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public decimal StartCash { get; set; }
    public decimal? EndCashCounted { get; set; }
    public decimal TotalSalesCash { get; set; }
    public decimal TotalSalesMomo { get; set; }
    public decimal TotalSalesVNPay { get; set; }
    public decimal TotalSalesBank { get; set; }
    public required string Status { get; set; } = "Open"; // "Open", "Closed"
    public string? Notes { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public ApplicationUser? User { get; set; }
}
