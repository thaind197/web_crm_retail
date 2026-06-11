using MediatR;
using SalesCRM.Application.DTOs.Product;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Products.Queries;

public record GetProductsQuery(
    string? SearchTerm,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<ApiResult<PagedResult<ProductDto>>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResult<PagedResult<ProductDto>>>
{
    private readonly IProductService _productService;

    public GetProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ApiResult<PagedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetPagedProductsAsync(
            request.SearchTerm,
            request.IsActive,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
