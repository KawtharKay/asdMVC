using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetAllCarts
    {
        public record GetAllCartQuery() : IRequest<Result<ICollection<GetAllCartResponse>>>;

        public class GetAllCartHandler : IRequestHandler<GetAllCartQuery, Result<ICollection<GetAllCartResponse>>>
        {
            private readonly ICartRepository _cartRepository;
            public GetAllCartHandler(ICartRepository cartRepository)
            {
                _cartRepository = cartRepository;
            }
            public async Task<Result<ICollection<GetAllCartResponse>>> Handle(GetAllCartQuery request, CancellationToken cancellationToken)
            {
                var response = await _cartRepository.GetAllCartsAsync();
                return Result<ICollection<GetAllCartResponse>>.Success("Success!", response.Adapt<ICollection<GetAllCartResponse>>());
            }
        }

        public record GetAllCartResponse(Guid Id, Guid CustomerId, decimal TotalPrice);
    }
}