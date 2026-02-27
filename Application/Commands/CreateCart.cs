using Application.Common;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateCart
    {
        public record CreateCartCommand(Guid CustomerId) : IRequest<Result<CreateCartResponse>>;

        public class CreateCartHandler : IRequestHandler<CreateCartCommand, Result<CreateCartResponse>>
        {
            private readonly ICartRepository _cartRepository;
            private readonly IUnitOfWork _unitOfWork;
            public CreateCartHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
            {
                _cartRepository = cartRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Result<CreateCartResponse>> Handle(CreateCartCommand request, CancellationToken cancellationToken)
            {
                var cartExist = await _cartRepository.GetCartByUserIdAsync(request.CustomerId);
                if (cartExist is null) throw new Exception("Cart already exists");

                var cart = new Cart
                {
                    CustomerId = request.CustomerId
                };

                await _cartRepository.AddCartAsync(cart);
                await _unitOfWork.SaveAsync();

                return Result<CreateCartResponse>.Success("Success!", cart.Adapt<CreateCartResponse>());
            }
        }

        public record CreateCartResponse(Guid Id);
    }
}