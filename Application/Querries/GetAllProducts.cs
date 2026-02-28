using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetProducts
    {
        public record GetProductsQuery : IRequest<Result<ICollection<GetProductsResponse>>>;

        public class GetProductsHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, Result<ICollection<GetProductsResponse>>>
        {
            public async Task<Result<ICollection<GetProductsResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
            {
                var products = await productRepository.GetProductsAsync();
                return Result<ICollection<GetProductsResponse>>.Success("Success!", products.Adapt<ICollection<GetProductsResponse>>());
            }
        }

        public record GetProductsResponse(Guid Id, string Name, string Sku, string ImageUrl, decimal Price, Guid CategoryId);
    }
}