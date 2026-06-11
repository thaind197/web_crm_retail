namespace SalesCRM.Application.Interfaces.Services;

public interface IVietQrService
{
    string GenerateQrUrl(string bankBin, string accountNumber, string accountName, decimal amount, string description);
}
