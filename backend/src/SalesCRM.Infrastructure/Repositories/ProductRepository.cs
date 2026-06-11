using Microsoft.EntityFrameworkCore;
using SalesCRM.Application.Interfaces.Repositories;
using SalesCRM.Domain.Entities;
using SalesCRM.Infrastructure.Data;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.Code.ToLower() == code.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsByCodeExcludeIdAsync(string code, Guid excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.Code.ToLower() == code.ToLower() && p.Id != excludeId, cancellationToken);
    }

    public async Task<PagedResult<Product>> GetPagedProductsAsync(
        string? searchTerm, 
        bool? isActive, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Code.ToLower().Contains(term) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(term)) ||
                (p.Description != null && p.Description.ToLower().Contains(term))
            );
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, pageNumber, pageSize, totalCount);
    }

    public async Task<bool> HasOrderOrInventoryDependenciesAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var hasOrders = await _context.Set<OrderDetail>()
            .AnyAsync(o => o.ProductId == productId, cancellationToken);

        var hasInventories = await _context.Set<BranchInventory>()
            .AnyAsync(i => i.ProductId == productId && i.Quantity > 0, cancellationToken);

        return hasOrders || hasInventories;
    }
}
