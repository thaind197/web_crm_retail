using MediatR;
using SalesCRM.Application.Interfaces.Services;
using SalesCRM.Shared.Models;

namespace SalesCRM.Application.Features.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<ApiResult>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResult>
{
    private readonly IProductService _productService;

    public DeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ApiResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.DeleteProductAsync(request.Id, cancellationToken);
    }
}
