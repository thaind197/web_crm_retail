using Microsoft.Extensions.Localization;
using SalesCRM.Application.DTOs.Product;
using SalesCRM.Application.Interfaces.Repositories;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Application.Resources;
using SalesCRM.Domain.Entities;
using SalesCRM.Shared.Models;

namespace SalesCRM.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public ProductService(IUnitOfWork unitOfWork, IStringLocalizer<SharedResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<ApiResult<PagedResult<ProductDto>>> GetPagedProductsAsync(
        string? searchTerm, 
        bool? isActive, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var pagedProducts = await _unitOfWork.Products.GetPagedProductsAsync(
            searchTerm, 
            isActive, 
            pageNumber, 
            pageSize, 
            cancellationToken);

        var dtoItems = pagedProducts.Items.Select(p => new ProductDto
        {
            Id = p.Id,
            Code = p.Code,
            Barcode = p.Barcode,
            Name = p.Name,
            Description = p.Description,
            SellingPrice = p.SellingPrice,
            ImportPrice = p.ImportPrice,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            LastModifiedAt = p.LastModifiedAt
        }).ToList();

        var pagedResult = new PagedResult<ProductDto>(dtoItems, pagedProducts.PageIndex, pagedProducts.PageSize, pagedProducts.TotalCount);
        return ApiResult<PagedResult<ProductDto>>.Success(pagedResult);
    }

    public async Task<ApiResult<ProductDto>> GetProductByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return ApiResult<ProductDto>.Failure(_localizer["ProductNotFound", id]);
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Barcode = product.Barcode,
            Name = product.Name,
            Description = product.Description,
            SellingPrice = product.SellingPrice,
            ImportPrice = product.ImportPrice,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            LastModifiedAt = product.LastModifiedAt
        };

        return ApiResult<ProductDto>.Success(dto);
    }

    public async Task<ApiResult<Guid>> CreateProductAsync(
        string? code, 
        string? barcode, 
        string name, 
        string? description, 
        decimal sellingPrice, 
        decimal importPrice, 
        string? imageUrl, 
        string? imageBase64, 
        bool isActive, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var spCodes = allProducts
                .Select(p => p.Code)
                .Where(c => c.StartsWith("SP", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Substring(2))
                .Select(s => int.TryParse(s, out var num) ? num : 0)
                .ToList();
            
            int nextNumber = spCodes.Any() ? spCodes.Max() + 1 : 1;
            code = $"SP{nextNumber:D3}";
        }

        // Check if unique code already exists
        var codeExists = await _unitOfWork.Products.ExistsByCodeAsync(code, cancellationToken);

        if (codeExists)
        {
            return ApiResult<Guid>.Failure(_localizer["ProductCodeExists", code]);
        }

        // Process Base64 image if provided
        string? processedImageUrl = imageUrl;
        if (!string.IsNullOrWhiteSpace(imageBase64))
        {
            try
            {
                var base64Data = imageBase64;
                var ext = "png";
                if (base64Data.Contains(","))
                {
                    var parts = base64Data.Split(',');
                    var header = parts[0];
                    base64Data = parts[1];

                    if (header.Contains("image/jpeg") || header.Contains("image/jpg")) ext = "jpg";
                    else if (header.Contains("image/gif")) ext = "gif";
                    else if (header.Contains("image/webp")) ext = "webp";
                }

                var bytes = Convert.FromBase64String(base64Data);
                
                // Define local upload folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}.{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
                
                processedImageUrl = $"/uploads/{fileName}";
            }
            catch (Exception ex)
            {
                return ApiResult<Guid>.Failure(_localizer["ProductImageSaveError", ex.Message]);
            }
        }

        var product = new Product
        {
            Code = code.Trim(),
            Barcode = barcode?.Trim(),
            Name = name.Trim(),
            Description = description?.Trim(),
            SellingPrice = sellingPrice,
            ImportPrice = importPrice,
            ImageUrl = processedImageUrl,
            IsActive = isActive
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Success(product.Id, _localizer["ProductCreateSuccess"]);
    }

    public async Task<ApiResult> UpdateProductAsync(
        Guid id, 
        string code, 
        string? barcode, 
        string name, 
        string? description, 
        decimal sellingPrice, 
        decimal importPrice, 
        string? imageUrl, 
        string? imageBase64, 
        bool isActive, 
        CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return ApiResult.Failure(_localizer["ProductNotFound", id]);
        }

        // Check if SKU is updated and conflict with another product
        var codeConflict = await _unitOfWork.Products.ExistsByCodeExcludeIdAsync(code, id, cancellationToken);

        if (codeConflict)
        {
            return ApiResult.Failure(_localizer["ProductCodeConflict", code]);
        }

        // Process Base64 image if provided
        string? processedImageUrl = imageUrl;
        if (!string.IsNullOrWhiteSpace(imageBase64))
        {
            try
            {
                var base64Data = imageBase64;
                var ext = "png";
                if (base64Data.Contains(","))
                {
                    var parts = base64Data.Split(',');
                    var header = parts[0];
                    base64Data = parts[1];

                    if (header.Contains("image/jpeg") || header.Contains("image/jpg")) ext = "jpg";
                    else if (header.Contains("image/gif")) ext = "gif";
                    else if (header.Contains("image/webp")) ext = "webp";
                }

                var bytes = Convert.FromBase64String(base64Data);
                
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}.{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
                
                processedImageUrl = $"/uploads/{fileName}";
            }
            catch (Exception ex)
            {
                return ApiResult.Failure(_localizer["ProductImageSaveError", ex.Message]);
            }
        }

        product.Code = code.Trim();
        product.Barcode = barcode?.Trim();
        product.Name = name.Trim();
        product.Description = description?.Trim();
        product.SellingPrice = sellingPrice;
        product.ImportPrice = importPrice;
        product.ImageUrl = processedImageUrl;
        product.IsActive = isActive;
        product.LastModifiedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Success(_localizer["ProductUpdateSuccess"]);
    }

    public async Task<ApiResult> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return ApiResult.Failure(_localizer["ProductNotFound", id]);
        }

        var hasDependencies = await _unitOfWork.Products.HasOrderOrInventoryDependenciesAsync(id, cancellationToken);

        if (hasDependencies)
        {
            // Soft delete
            product.IsActive = false;
            product.LastModifiedAt = DateTime.UtcNow;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return ApiResult.Success(_localizer["ProductDeleteSoftSuccess"]);
        }

        _unitOfWork.Products.Delete(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return ApiResult.Success(_localizer["ProductDeleteSuccess"]);
    }
}
