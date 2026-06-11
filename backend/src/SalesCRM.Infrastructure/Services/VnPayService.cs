using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SalesCRM.Application.Interfaces.Services;

namespace SalesCRM.Infrastructure.Services;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;

    public VnPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> CreatePaymentUrlAsync(Guid orderId, decimal amount, string ipAddress, CancellationToken cancellationToken = default)
    {
        var vnpayConfig = _configuration.GetSection("VNPAY");
        var tmnCode = vnpayConfig["TmnCode"] ?? "SALESCRM";
        var hashSecret = vnpayConfig["HashSecret"] ?? "SuperSecretVNPAYHashSecretKeyMustBe32Bytes!";
        var paymentUrl = vnpayConfig["PaymentUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        var returnUrl = vnpayConfig["ReturnUrl"] ?? "http://localhost:5173/payment-callback/vnpay";

        var vnpParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Amount", ((long)(amount * 100)).ToString() },
            { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", $"Thanh toan don hang {orderId}" },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_TxnRef", orderId.ToString() + "_" + DateTime.UtcNow.Ticks }
        };

        // Construct raw data string
        var rawDataBuilder = new StringBuilder();
        var queryBuilder = new StringBuilder();
        foreach (var kvp in vnpParams)
        {
            if (rawDataBuilder.Length > 0)
            {
                rawDataBuilder.Append("&");
                queryBuilder.Append("&");
            }
            rawDataBuilder.Append(kvp.Key + "=" + kvp.Value);
            queryBuilder.Append(Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value));
        }

        // Calculate SecureHash
        var secureHash = HmacSha512(hashSecret, rawDataBuilder.ToString());
        queryBuilder.Append("&vnp_SecureHash=" + secureHash);

        var finalUrl = paymentUrl + "?" + queryBuilder.ToString();
        return Task.FromResult(finalUrl);
    }

    private static string HmacSha512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashValue = hmac.ComputeHash(inputBytes);
        return Convert.ToHexString(hashValue).ToLower();
    }
}
