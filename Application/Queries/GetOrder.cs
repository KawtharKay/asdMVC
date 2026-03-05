using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetOrderById
    {
        public record GetOrderByIdQuery(Guid OrderId)
            : IRequest<Result<GetOrderByIdResponse>>;

        public class GetOrderByIdValidator : AbstractValidator<GetOrderByIdQuery>
        {
            public GetOrderByIdValidator()
            {
                RuleFor(x => x.OrderId)
                    .NotEmpty().WithMessage("Order ID is required");
            }
        }

        public class GetOrderByIdHandler(IOrderRepository orderRepository)
            : IRequestHandler<GetOrderByIdQuery, Result<GetOrderByIdResponse>>
        {
            public async Task<Result<GetOrderByIdResponse>> Handle(
                GetOrderByIdQuery request,
                CancellationToken cancellationToken)
            {
                var o = await orderRepository.GetByIdAsync(request.OrderId);
                if (o is null)
                    return Result<GetOrderByIdResponse>.Failure("Order not found");

                return Result<GetOrderByIdResponse>.Success(
                    "Success",
                    new GetOrderByIdResponse(
                        o.Id, o.OrderNo, o.TotalAmount,
                        o.Status.ToString(), o.DeliveryAddress, o.DateCreated,
                        o.ProductOrders.Select(po => new OrderItemResponse(
                            po.ProductId, po.Product.Name,
                            po.Product.ImageUrl, po.Quantity,
                            po.UnitPrice)).ToList()));
            }
        }

        public record GetOrderByIdResponse(
            Guid OrderId, string OrderNo, decimal TotalAmount,
            string Status, string DeliveryAddress,
            DateTime DateCreated, List<OrderItemResponse> Items);

        public record OrderItemResponse(
            Guid ProductId, string ProductName,
            string ImageUrl, int Quantity, decimal UnitPrice);
    }
}