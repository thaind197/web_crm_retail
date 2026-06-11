using SalesCRM.Application.DTOs.Product;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Interfaces.Services;

public interface IProductService
{
    Task<ApiResult<PagedResult<ProductDto>>> GetPagedProductsAsync(string? searchTerm, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<ProductDto>> GetProductByIdAsync(Guid id);
    Task<ApiResult<Guid>> CreateProductAsync(string? code, string? barcode, string name, string? description, decimal sellingPrice, decimal importPrice, string? imageUrl, string? imageBase64, bool isActive, CancellationToken cancellationToken = default);
    Task<ApiResult> UpdateProductAsync(Guid id, string code, string? barcode, string name, string? description, decimal sellingPrice, decimal importPrice, string? imageUrl, string? imageBase64, bool isActive, CancellationToken cancellationToken = default);
    Task<ApiResult> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
