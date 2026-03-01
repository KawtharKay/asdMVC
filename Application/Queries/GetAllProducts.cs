using Application.Common.Dtos;
using Application.Repositories;
using MediatR;

namespace Application.Queries
{
    public class GetAllProducts
    {
        public record GetAllProductsQuery(
            Guid? CategoryId = null)
            : IRequest<Result<List<GetAllProductsResponse>>>;

        public class GetAllProductsHandler(
            IProductRepository productRepository)
            : IRequestHandler<GetAllProductsQuery,
                Result<List<GetAllProductsResponse>>>
        {
            public async Task<Result<List<GetAllProductsResponse>>> Handle(
                GetAllProductsQuery request,
                CancellationToken cancellationToken)
            {
                var products = request.CategoryId.HasValue
                    ? await productRepository.GetByCategoryIdAsync(request.CategoryId.Value)
                    : await productRepository.GetAllAsync();

                var response = products
                    .Where(p => !p.IsDeleted)
                    .Select(p => new GetAllProductsResponse(
                        p.Id, p.Name, p.Sku,
                        p.Price, p.ImageUrl,
                        p.QrCodeImagePath,
                        p.StockQuantity,
                        p.IsInStock,
                        p.IsLowStock,
                        p.Category.Name))
                    .ToList();

                return Result<List<GetAllProductsResponse>>.Success("Success", response);
            }
        }

        public record GetAllProductsResponse(
            Guid Id,
            string Name,
            string Sku,
            decimal Price,
            string ImageUrl,
            string? QrCodeImagePath,
            int StockQuantity,
            bool IsInStock,
            bool IsLowStock,
            string CategoryName);
    }
}