using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class UpdateProduct
    {
        public record UpdateProductCommand(
            Guid Id,
            string Name,
            string ImageUrl,
            decimal Price,
            Guid CategoryId,
            int StockQuantity,
            int LowStockThreshold) : IRequest<Result<string>>;

        public class UpdateProductValidator
            : AbstractValidator<UpdateProductCommand>
        {
            public UpdateProductValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Product ID is required");

                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Product name is required")
                    .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

                RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than zero");

                RuleFor(x => x.CategoryId)
                    .NotEmpty().WithMessage("Category is required");

                RuleFor(x => x.StockQuantity)
                    .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

                RuleFor(x => x.LowStockThreshold)
                    .GreaterThanOrEqualTo(0).WithMessage("Threshold cannot be negative");
            }
        }

        public class UpdateProductHandler(
            IProductRepository productRepository,
            IQrCodeService qrCodeService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateProductCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateProductCommand request,
                CancellationToken cancellationToken)
            {
                var product = await productRepository.GetByIdAsync(request.Id);
                if (product is null)
                    return Result<string>.Failure("Product not found");

                product.Name = request.Name;
                product.ImageUrl = request.ImageUrl;
                product.Price = request.Price;
                product.CategoryId = request.CategoryId;
                product.StockQuantity = request.StockQuantity;
                product.LowStockThreshold = request.LowStockThreshold;
                product.DateModified = DateTime.UtcNow;

                var qrContent = $"ID:{product.Id}|Name:{product.Name}|SKU:{product.Sku}|Price:₦{product.Price:N2}";
                product.QrCodeImagePath = qrCodeService
                    .GenerateQrCodeImage(qrContent, product.Id.ToString());

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Product updated successfully!", "updated");
            }
        }
    }
}