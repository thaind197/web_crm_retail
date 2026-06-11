using System;

namespace SalesCRM.Domain.Entities;

public class ReturnOrderItem : BaseEntity
{
    public Guid ReturnOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal RefundPrice { get; set; }

    // Navigation properties
    public ReturnOrder? ReturnOrder { get; set; }
    public Product? Product { get; set; }
}
