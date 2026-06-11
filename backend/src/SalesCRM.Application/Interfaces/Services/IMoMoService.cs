namespace SalesCRM.Application.Interfaces.Services;

public interface IMoMoService
{
    Task<string> CreatePaymentUrlAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}
