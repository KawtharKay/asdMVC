using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class AddToCart
    {
        public record AddToCartCommand(
            Guid CustomerId,
            Guid ProductId,
            int Quantity) : IRequest<Result<string>>;

        public class AddToCartValidator : AbstractValidator<AddToCartCommand>
        {
            public AddToCartValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");

                RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Product ID is required");

                RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1");
            }
        }

        public class AddToCartHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddToCartCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
            {
                var product = await productRepository.GetByIdAsync(request.ProductId);
                if (product is null) return Result<string>.Failure("Product not found");

                if (!product.IsInStock) return Result<string>.Failure("Product is out of stock");

                if (product.StockQuantity < request.Quantity) return Result<string>.Failure($"Only {product.StockQuantity} items available");

                var cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId);
                if (cart is null) return Result<string>.Failure("Cart not found");

                var existingItem = await cartItemRepository.GetByCartAndProductAsync(cart.Id, request.ProductId);

                if (existingItem is not null)
                {
                    existingItem.Quantity += request.Quantity;
                    existingItem.DateModified = DateTime.UtcNow;
                }
                else
                {
                    await cartItemRepository.AddAsync(new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        CreatedBy = request.CustomerId.ToString()
                    });
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Item added to cart!", "added");
            }
        }
    }
}