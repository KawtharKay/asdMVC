using Application.Common.Dtos;
using Application.Repositories;
using MediatR;

namespace Application.Queries
{
    public class GetAllOrders
    {
        public record GetAllOrdersQuery()
            : IRequest<Result<List<GetAllOrdersResponse>>>;

        public class GetAllOrdersHandler(IOrderRepository orderRepository)
            : IRequestHandler<GetAllOrdersQuery,
                Result<List<GetAllOrdersResponse>>>
        {
            public async Task<Result<List<GetAllOrdersResponse>>> Handle(
                GetAllOrdersQuery request,
                CancellationToken cancellationToken)
            {
                var orders = await orderRepository.GetAllAsync();

                var response = orders.Select(o => new GetAllOrdersResponse(
                    o.Id, o.OrderNo,
                    o.Customer.Name,
                    o.Customer.Email,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.DateCreated))
                    .ToList();

                return Result<List<GetAllOrdersResponse>>.Success("Success", response);
            }
        }

        public record GetAllOrdersResponse(
            Guid OrderId, string OrderNo,
            string CustomerName, string CustomerEmail,
            decimal TotalAmount, string Status,
            DateTime DateCreated);
    }
}