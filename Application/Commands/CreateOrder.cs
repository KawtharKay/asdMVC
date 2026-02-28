using Application.Common.Dtos;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateOrder
    {
        public record CreateOrderCommand(string OrderNo, Guid CustomerId, decimal TotalAmount, string DeliveryAddress) : IRequest<Result<CreateOrderResponse>>;

        public class CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
        {
            public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            {
                var orderExists = await orderRepository.IsExistAsync(request.OrderNo);
                if (orderExists) throw new Exception("Order already exists");

                var order = new Order
                {
                    OrderNo = request.OrderNo,
                    CustomerId = request.CustomerId,
                    TotalAmount = request.TotalAmount,
                    DeliveryAddress = request.DeliveryAddress,
                    Status = OrderStatus.Pending
                };

                await orderRepository.AddToDbAsync(order);
                await unitOfWork.SaveAsync();

                return Result<CreateOrderResponse>.Success("Success!", order.Adapt<CreateOrderResponse>());
            }
        }

        public record CreateOrderResponse(Guid Id);
    }
}