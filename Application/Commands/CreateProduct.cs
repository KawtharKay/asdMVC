using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateProduct
    {
        public record CreateProductCommand(
            string Name,
            string Sku,
            string ImageUrl,
            decimal Price,
            Guid CategoryId,
            int StockQuantity,
            int LowStockThreshold) : IRequest<Result<CreateProductResponse>>;

        public class CreateProductHandler(
            IProductRepository productRepository,
            IQrCodeService qrCodeService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
        {
            public async Task<Result<CreateProductResponse>> Handle(
                CreateProductCommand request,
                CancellationToken cancellationToken)
            {
                var productExist = await productRepository.IsExistAsync(request.Sku);
                if (productExist)
                    return Result<CreateProductResponse>.Failure("Product already exists");

                var product = new Product
                {
                    Name = request.Name,
                    Sku = request.Sku,
                    ImageUrl = request.ImageUrl,
                    Price = request.Price,
                    CategoryId = request.CategoryId,
                    StockQuantity = request.StockQuantity,
                    LowStockThreshold = request.LowStockThreshold,
                    CreatedBy = "Admin"
                };

                await productRepository.AddToDbAsync(product);
                await unitOfWork.SaveAsync();

                // Generate QR code after save so we have the product ID
                var qrContent = $"ID:{product.Id}|Name:{product.Name}|SKU:{product.Sku}|Price:₦{product.Price:N2}";
                product.QrCodeImagePath = qrCodeService.GenerateQrCodeImage(
                    qrContent, product.Id.ToString());

                await unitOfWork.SaveAsync();

                return Result<CreateProductResponse>.Success(
                    "Product created successfully!",
                    product.Adapt<CreateProductResponse>());
            }
        }

        public record CreateProductResponse(Guid Id);
    }
}