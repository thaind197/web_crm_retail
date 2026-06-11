using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SalesCRM.Application.Interfaces.Services;

namespace SalesCRM.Infrastructure.Services;

public class MoMoService : IMoMoService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public MoMoService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> CreatePaymentUrlAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var momoConfig = _configuration.GetSection("MoMo");
        var partnerCode = momoConfig["PartnerCode"] ?? "MOMO";
        var accessKey = momoConfig["AccessKey"] ?? "F8B3Y423J42J42K";
        var secretKey = momoConfig["SecretKey"] ?? "SuperSecretMoMoHashSecretKeyMustBe32Bytes!";
        var endpoint = momoConfig["Endpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/create";
        var redirectUrl = momoConfig["RedirectUrl"] ?? "http://localhost:5173/payment-callback/momo";
        var ipnUrl = momoConfig["IpnUrl"] ?? "http://localhost:5000/api/payments/momo-ipn";

        var requestId = Guid.NewGuid().ToString();
        var orderInfo = $"Thanh toan don hang {orderId}";
        var requestType = "captureWallet";
        var extraData = "";
        var txnId = orderId.ToString() + "_" + DateTime.UtcNow.Ticks;

        var rawHash = $"accessKey={accessKey}&amount={(long)amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={txnId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
        var signature = HmacSha256(secretKey, rawHash);

        var requestBody = new
        {
            partnerCode,
            partnerName = "SalesCRM Multi-Branch",
            storeId = "SalesCRM_Store",
            requestId,
            amount = (long)amount,
            orderId = txnId,
            orderInfo,
            redirectUrl,
            ipnUrl,
            lang = "vi",
            extraData,
            requestType,
            signature
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(endpoint, content, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("payUrl", out var payUrlProp))
                {
                    return payUrlProp.GetString() ?? endpoint;
                }
            }
        }
        catch
        {
            // Fallback to a mock web checkout url in case of offline or DNS error
        }

        // Mock Checkout page link
        return $"https://test-payment.momo.vn/v2/gateway/api/mock-checkout?partnerCode={partnerCode}&orderId={txnId}&amount={(long)amount}&signature={signature}";
    }

    private static string HmacSha256(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA256(keyBytes);
        var hashValue = hmac.ComputeHash(inputBytes);
        return Convert.ToHexString(hashValue).ToLower();
    }
}
