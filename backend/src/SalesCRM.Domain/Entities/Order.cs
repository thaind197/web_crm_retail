using SalesCRM.Domain.Enums;

namespace SalesCRM.Domain.Entities;

public class Order : BaseEntity
{
    public Guid BranchId { get; set; }
    public required string OrderCode { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public Guid? ShiftId { get; set; }
    public string? CouponCode { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public Customer? Customer { get; set; }
    public Shift? Shift { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
