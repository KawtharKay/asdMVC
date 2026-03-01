using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class GetCart
    {
        public record GetCartQuery(Guid CustomerId)
            : IRequest<Result<GetCartResponse>>;

        public class GetCartValidator : AbstractValidator<GetCartQuery>
        {
            public GetCartValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetCartHandler(ICartRepository cartRepository)
            : IRequestHandler<GetCartQuery, Result<GetCartResponse>>
        {
            public async Task<Result<GetCartResponse>> Handle(
                GetCartQuery request,
                CancellationToken cancellationToken)
            {
                var cart = await cartRepository.GetByCustomerIdAsync(request.CustomerId);
                if (cart is null)
                    return Result<GetCartResponse>.Failure("Cart not found");

                var items = cart.CartItems
                    .Where(ci => !ci.IsDeleted)
                    .Select(ci => new CartItemResponse(
                        ci.Id,
                        ci.ProductId,
                        ci.Product.Name,
                        ci.Product.ImageUrl,
                        ci.Product.Price,
                        ci.Quantity,
                        ci.Quantity * ci.Product.Price))
                    .ToList();

                return Result<GetCartResponse>.Success(
                    "Success",
                    new GetCartResponse(
                        cart.Id, items, items.Sum(i => i.SubTotal)));
            }
        }

        public record GetCartResponse(
            Guid CartId,
            List<CartItemResponse> Items,
            decimal TotalPrice);

        public record CartItemResponse(
            Guid CartItemId,
            Guid ProductId,
            string ProductName,
            string ImageUrl,
            decimal UnitPrice,
            int Quantity,
            decimal SubTotal);
    }
}