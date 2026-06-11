namespace SalesCRM.Application.DTOs.POS;

public class PaymentResponseDto
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string? PaymentUrl { get; set; } // For VNPAY, MoMo, VietQR
    public string? VietQrPayload { get; set; }
    public bool IsCompleted { get; set; }
    public string? Message { get; set; }
}
