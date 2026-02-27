using Application.Common;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateProduct
    {
        public record CreateProductCommand(string Name, string Sku, string ImageUrl, decimal Price, Guid CategoryId) : IRequest<Result<CreateProductResponse>>;
        public class CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
        {
            public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
            {
                var productExist = await productRepository.IsExistAsync(request.Sku);
                if (productExist) throw new Exception("Product already exist");

                var product = new Product
                {
                    Name = request.Name,
                    Sku = request.Sku,
                    ImageUrl = request.ImageUrl,
                    Price = request.Price,
                    CategoryId = request.CategoryId
                };
                await productRepository.AddToDbAsync(product);
                await unitOfWork.SaveAsync();
                return Result<CreateProductResponse>.Success("Success!", product.Adapt<CreateProductResponse>());
            }
        }

        public record CreateProductResponse(Guid Id);
    }
}
