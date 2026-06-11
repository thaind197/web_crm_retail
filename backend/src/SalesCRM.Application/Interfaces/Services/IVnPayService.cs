namespace SalesCRM.Application.Interfaces.Services;

public interface IVnPayService
{
    Task<string> CreatePaymentUrlAsync(Guid orderId, decimal amount, string ipAddress, CancellationToken cancellationToken = default);
}
