using SalesCRM.Application.Interfaces.Services;

namespace SalesCRM.Infrastructure.Services;

public class VietQrService : IVietQrService
{
    public string GenerateQrUrl(string bankBin, string accountNumber, string accountName, decimal amount, string description)
    {
        return $"https://img.vietqr.io/image/{bankBin}-{accountNumber}-compact.png?amount={(int)amount}&addInfo={Uri.EscapeDataString(description)}&accountName={Uri.EscapeDataString(accountName)}";
    }
}
