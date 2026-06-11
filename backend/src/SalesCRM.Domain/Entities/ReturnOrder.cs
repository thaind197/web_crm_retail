using System;
using System.Collections.Generic;

namespace SalesCRM.Domain.Entities;

public class ReturnOrder : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid OriginalOrderId { get; set; }
    public required string ReturnCode { get; set; }
    public decimal RefundAmount { get; set; }
    public required string RefundMethod { get; set; } = "Cash"; // "Cash", "Momo", "VNPay"
    public string? Reason { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public Order? OriginalOrder { get; set; }
    public ICollection<ReturnOrderItem> ReturnItems { get; set; } = [];
}
