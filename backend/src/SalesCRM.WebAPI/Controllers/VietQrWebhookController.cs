using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.Infrastructure.Data;
using SalesCRM.WebAPI.Hubs;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SalesCRM.WebAPI.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/payments")]
public class VietQrWebhookController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<VietQrWebhookController> _logger;

    public VietQrWebhookController(
        ApplicationDbContext dbContext,
        IHubContext<PaymentHub> hubContext,
        ILogger<VietQrWebhookController> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("vietqr-webhook")]
    public async Task<IActionResult> ProcessWebhook([FromBody] JsonElement payload)
    {
        _logger.LogInformation("Received payment webhook: {Payload}", payload.GetRawText());

        string? description = null;
        decimal amount = 0;
        string? reference = null;

        try
        {
            // Support PayOS format
            if (payload.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Object)
            {
                if (dataProp.TryGetProperty("description", out var descProp)) description = descProp.GetString();
                if (dataProp.TryGetProperty("amount", out var amountProp)) amount = amountProp.GetDecimal();
                if (dataProp.TryGetProperty("reference", out var refProp)) reference = refProp.GetString();
            }
            // Support Casso format
            else if (payload.TryGetProperty("error", out var errProp) && payload.TryGetProperty("data", out var listProp) && listProp.ValueKind == JsonValueKind.Array)
            {
                var firstTx = listProp.EnumerateArray().FirstOrDefault();
                if (firstTx.ValueKind == JsonValueKind.Object)
                {
                    if (firstTx.TryGetProperty("description", out var dProp)) description = dProp.GetString();
                    if (firstTx.TryGetProperty("amount", out var aProp)) amount = aProp.GetDecimal();
                    if (firstTx.TryGetProperty("tid", out var tProp)) reference = tProp.GetString();
                }
            }
            // Fallback generic format
            else
            {
                if (payload.TryGetProperty("description", out var dProp)) description = dProp.GetString();
                if (payload.TryGetProperty("amount", out var aProp)) amount = aProp.GetDecimal();
                if (payload.TryGetProperty("reference", out var rProp)) reference = rProp.GetString();
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest(new { isSuccess = false, message = "Không tìm thấy nội dung giao dịch." });
            }

            // Extract Order Code matching pattern ORD\d+ (e.g. ORD20260603001)
            var match = Regex.Match(description, @"ORD[A-Za-z0-9_-]+", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return BadRequest(new { isSuccess = false, message = "Không tìm thấy mã đơn hàng trong nội dung giao dịch." });
            }

            string orderCode = match.Value.ToUpper();
            _logger.LogInformation("Processing payment for OrderCode: {OrderCode}, Amount: {Amount}", orderCode, amount);

            // Bypass multi-tenant global query filter to verify webhook matches across branches
            var order = await _dbContext.Orders
                .Include(o => o.Payments)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                return NotFound(new { isSuccess = false, message = $"Không tìm thấy đơn hàng {orderCode}." });
            }

            if (order.Status == OrderStatus.Paid)
            {
                return Ok(new { isSuccess = true, message = "Đơn hàng đã được thanh toán trước đó." });
            }

            // Update order status
            order.Status = OrderStatus.Paid;

            // Update payment record or add new one
            var payment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending)
                          ?? new Payment
                          {
                              OrderId = order.Id,
                              PaymentMethod = PaymentMethod.BankTransfer,
                              Amount = order.FinalAmount
                          };

            payment.Status = PaymentStatus.Completed;
            payment.TransactionNo = reference ?? "AUTO_VIETQR_" + DateTime.UtcNow.Ticks;
            payment.PaymentPayload = payload.GetRawText();

            if (payment.Id == Guid.Empty)
            {
                _dbContext.Payments.Add(payment);
            }

            if (order.ShiftId.HasValue)
            {
                var shift = await _dbContext.Shifts.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == order.ShiftId.Value);
                if (shift != null && shift.Status == "Open")
                {
                    if (payment.PaymentMethod == PaymentMethod.Cash)
                        shift.TotalSalesCash += payment.Amount;
                    else if (payment.PaymentMethod == PaymentMethod.MoMo)
                        shift.TotalSalesMomo += payment.Amount;
                    else if (payment.PaymentMethod == PaymentMethod.VNPAY)
                        shift.TotalSalesVNPay += payment.Amount;
                    else if (payment.PaymentMethod == PaymentMethod.BankTransfer)
                        shift.TotalSalesBank += payment.Amount;
                    
                    _dbContext.Shifts.Update(shift);
                }
            }

            await _dbContext.SaveChangesAsync();

            // Broadcast SignalR notification to frontend client
            await _hubContext.Clients.All.SendAsync("OrderPaid", new
            {
                orderId = order.Id,
                orderCode = order.OrderCode,
                status = "Paid"
            });

            return Ok(new { isSuccess = true, message = $"Thanh toán đơn hàng {orderCode} thành công." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý VietQR Webhook");
            return StatusCode(500, new { isSuccess = false, message = "Lỗi xử lý nội bộ.", error = ex.Message });
        }
    }
}
