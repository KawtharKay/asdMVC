using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FluentValidation;
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

        public class CreateProductValidator
            : AbstractValidator<CreateProductCommand>
        {
            public CreateProductValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Product name is required")
                    .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

                RuleFor(x => x.Sku)
                    .NotEmpty().WithMessage("SKU is required")
                    .MaximumLength(100).WithMessage("SKU cannot exceed 100 characters");

                RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than zero");

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("Category is required");

                RuleFor(x => x.StockQuantity)
                    .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

                RuleFor(x => x.LowStockThreshold)
                    .GreaterThanOrEqualTo(0).WithMessage("Threshold cannot be negative");

                RuleFor(x => x.ImageUrl)
                    .NotEmpty().WithMessage("Product image is required");
            }
        }

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
                    return Result<CreateProductResponse>.Failure("Product SKU already exists");

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

                var qrContent = $"ID:{product.Id}|Name:{product.Name}|SKU:{product.Sku}|Price:₦{product.Price:N2}";
                product.QrCodeImagePath = qrCodeService
                    .GenerateQrCodeImage(qrContent, product.Id.ToString());

                await unitOfWork.SaveAsync();

                return Result<CreateProductResponse>.Success(
                    "Product created successfully!",
                    product.Adapt<CreateProductResponse>());
            }
        }

        public record CreateProductResponse(Guid Id);
    }
}