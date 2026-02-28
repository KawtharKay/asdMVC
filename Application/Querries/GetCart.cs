using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetCart
    {
        public record GetCartQuery(Guid Id) : IRequest<Result<GetCartResponse>>;

        public class GetCartHandler : IRequestHandler<GetCartQuery, Result<GetCartResponse>>
        {
            private readonly ICartRepository _cartRepository;
            public GetCartHandler(ICartRepository cartRepository)
            {
                _cartRepository = cartRepository;
            }
            public async Task<Result<GetCartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
            {
                var cart = await _cartRepository.GetCartByIdAsync(request.Id);
                if (cart is null) throw new Exception("Cart not found");

                return Result<GetCartResponse>.Success("Success!", cart.Adapt<GetCartResponse>());
            }
        }

        public record GetCartResponse(Guid Id, Guid CustomerId, decimal TotalPrice);
    }
}