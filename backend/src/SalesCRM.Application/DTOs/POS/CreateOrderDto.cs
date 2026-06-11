using SalesCRM.Domain.Enums;

namespace SalesCRM.Application.DTOs.POS;

public class CreateOrderDto
{
    public Guid? CustomerId { get; set; }
    public decimal Discount { get; set; }
    public string? CouponCode { get; set; }
    public List<CreateOrderDetailDto> Items { get; set; } = [];
    
    // Optional immediate payment fields
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal? PaymentAmount { get; set; }
}

public class CreateOrderDetailDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductBatchId { get; set; } // Specific batch to deduct from (optional)
    public int Quantity { get; set; }
}
