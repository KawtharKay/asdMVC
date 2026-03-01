using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetMyOrders
    {
        public record GetMyOrdersQuery(Guid CustomerId)
            : IRequest<Result<List<GetMyOrdersResponse>>>;

        public class GetMyOrdersValidator : AbstractValidator<GetMyOrdersQuery>
        {
            public GetMyOrdersValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetMyOrdersHandler(IOrderRepository orderRepository)
            : IRequestHandler<GetMyOrdersQuery,
                Result<List<GetMyOrdersResponse>>>
        {
            public async Task<Result<List<GetMyOrdersResponse>>> Handle(
                GetMyOrdersQuery request,
                CancellationToken cancellationToken)
            {
                var orders = await orderRepository
                    .GetByCustomerIdAsync(request.CustomerId);

                var response = orders.Select(o => new GetMyOrdersResponse(
                    o.Id, o.OrderNo, o.TotalAmount,
                    o.Status.ToString(), o.DeliveryAddress, o.DateCreated,
                    o.ProductOrders.Select(po => new OrderItemResponse(
                        po.ProductId, po.Product.Name,
                        po.Product.ImageUrl, po.Quantity,
                        po.UnitPrice)).ToList()))
                    .ToList();

                return Result<List<GetMyOrdersResponse>>.Success("Success", response);
            }
        }

        public record GetMyOrdersResponse(
            Guid OrderId, string OrderNo, decimal TotalAmount,
            string Status, string DeliveryAddress,
            DateTime DateCreated, List<OrderItemResponse> Items);

        public record OrderItemResponse(
            Guid ProductId, string ProductName,
            string ImageUrl, int Quantity, decimal UnitPrice);
    }
}