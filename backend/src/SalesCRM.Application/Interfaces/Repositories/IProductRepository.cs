using SalesCRM.Domain.Entities;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetPagedProductsAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> HasOrderOrInventoryDependenciesAsync(Guid productId, CancellationToken cancellationToken = default);
}
