using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class UpdateCartItem
    {
        public record UpdateCartItemCommand(
            Guid CartItemId,
            int Quantity) : IRequest<Result<string>>;

        public class UpdateCartItemValidator
            : AbstractValidator<UpdateCartItemCommand>
        {
            public UpdateCartItemValidator()
            {
                RuleFor(x => x.CartItemId)
                    .NotEmpty().WithMessage("Cart item ID is required");

                RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1");
            }
        }

        public class UpdateCartItemHandler(
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateCartItemCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateCartItemCommand request,
                CancellationToken cancellationToken)
            {
                var cartItem = await cartItemRepository.GetByIdAsync(request.CartItemId);
                if (cartItem is null)
                    return Result<string>.Failure("Cart item not found");

                var product = await productRepository.GetByIdAsync(cartItem.ProductId);
                if (product is null)
                    return Result<string>.Failure("Product not found");

                if (product.StockQuantity < request.Quantity)
                    return Result<string>.Failure(
                        $"Only {product.StockQuantity} items available");

                cartItem.Quantity = request.Quantity;
                cartItem.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Cart updated!", "updated");
            }
        }
    }
}