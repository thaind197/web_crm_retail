using SalesCRM.Domain.Enums;

namespace SalesCRM.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionNo { get; set; } // Mã tham chiếu giao dịch (MoMo, VNPAY, Bank)
    public string? PaymentPayload { get; set; } // Dữ liệu phản hồi thô phục vụ đối soát

    // Navigation property
    public Order? Order { get; set; }
}
