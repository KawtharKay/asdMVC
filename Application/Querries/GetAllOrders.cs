using Application.Common.Dtos;
using Application.Repositories;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Querries
{
    public class GetAllOrders
    {
        public record GetAllOrderQuery() : IRequest<Result<ICollection<GetAllOrderResponse>>>;
        public class GetAllOrderHandler(IOrderRepository orderRepository) : IRequestHandler<GetAllOrderQuery, Result<ICollection<GetAllOrderResponse>>>
{
            public async Task<Result<ICollection<GetAllOrderResponse>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
            {
                var response = await orderRepository.GetOrdersAsync();
                return Result<ICollection<GetAllOrderResponse>>.Success("Success!", response.Adapt<ICollection<GetAllOrderResponse>>());
            }
        }

        public record GetAllOrderResponse(Guid Id, string OrderNo, Guid CustomerId, decimal TotalAmount, OrderStatus Status, string DeliveryAddress);
    }
}
