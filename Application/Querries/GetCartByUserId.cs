using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetCartByUserId
    {
        public record GetCartByUserIdQuery(Guid UserId) : IRequest<Result<GetCartByUserIdResponse>>;

        public class GetCartByUserIdHandler : IRequestHandler<GetCartByUserIdQuery, Result<GetCartByUserIdResponse>>
        {
            private readonly ICartRepository _cartRepository;
            public GetCartByUserIdHandler(ICartRepository cartRepository)
            {
                _cartRepository = cartRepository;
            }
            public async Task<Result<GetCartByUserIdResponse>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(request.UserId);
                if (cart is null) throw new Exception("Cart not found");

                return Result<GetCartByUserIdResponse>.Success("Success!", cart.Adapt<GetCartByUserIdResponse>());
            }
        }

        public record GetCartByUserIdResponse(Guid Id, Guid CustomerId, decimal TotalPrice);
    }
}