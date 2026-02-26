using Application.Common;
using Application.Repositories;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Querries
{
    public class GetOrder
    {
        public record GetOrderQuery(Guid Id) : IRequest<Result<GetOrderResponse>>;
        public class GetOrderHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderQuery, Result<GetOrderResponse>>
        {
            public async Task<Result<GetOrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
            {
                var response = await orderRepository.GetOrderAsync(request.Id);
                if (response is null) throw new Exception("Order not found");

                return Result<GetOrderResponse>.Success("Success!", response.Adapt<GetOrderResponse>());
            }
        }

        public record GetOrderResponse(Guid Id, string OrderNo, Guid CustomerId, decimal TotalAmount, OrderStatus Status, string DeliveryAddress);
    }
}
