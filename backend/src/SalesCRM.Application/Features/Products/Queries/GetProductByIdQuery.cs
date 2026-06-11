using MediatR;
using SalesCRM.Application.DTOs.Product;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ApiResult<ProductDto>>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResult<ProductDto>>
{
    private readonly IProductService _productService;

    public GetProductByIdQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ApiResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _productService.GetProductByIdAsync(request.Id);
    }
}
