using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetProduct
    {
        public record GetProductQuery(Guid Id) : IRequest<Result<GetProductResponse>>;

        public class GetProductHandler(IProductRepository productRepository) : IRequestHandler<GetProductQuery, Result<GetProductResponse>>
        {
            public async Task<Result<GetProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
            {
                var product = await productRepository.GetProductAsync(request.Id);
                if (product is null) throw new Exception("Product not found");

                return Result<GetProductResponse>.Success("Success!", product.Adapt<GetProductResponse>());
            }
        }

        public record GetProductResponse(Guid Id, string Name, string Sku, string ImageUrl, decimal Price, Guid CategoryId);
    }
}