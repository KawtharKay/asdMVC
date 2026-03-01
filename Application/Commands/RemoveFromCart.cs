using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class RemoveFromCart
    {
        public record RemoveFromCartCommand(
            Guid CartItemId) : IRequest<Result<string>>;

        public class RemoveFromCartValidator
            : AbstractValidator<RemoveFromCartCommand>
        {
            public RemoveFromCartValidator()
            {
                RuleFor(x => x.CartItemId)
                    .NotEmpty().WithMessage("Cart item ID is required");
            }
        }

        public class RemoveFromCartHandler(
            ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RemoveFromCartCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                RemoveFromCartCommand request,
                CancellationToken cancellationToken)
            {
                var cartItem = await cartItemRepository.GetByIdAsync(request.CartItemId);
                if (cartItem is null)
                    return Result<string>.Failure("Cart item not found");

                cartItem.IsDeleted = true;
                cartItem.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Item removed from cart!", "removed");
            }
        }
    }
}