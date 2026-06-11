using SalesCRM.Domain.Enums;

namespace SalesCRM.Application.DTOs.POS;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string? BranchName { get; set; }
    public required string OrderCode { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string? CouponCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = [];
    public List<PaymentDto> Payments { get; set; } = [];
}

public class OrderDetailDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public Guid? ProductBatchId { get; set; }
    public string? BatchCode { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionNo { get; set; }
    public DateTime CreatedAt { get; set; }
}
