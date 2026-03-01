using Application.Common.Dtos;
using Application.Repositories;
using MediatR;

namespace Application.Queries
{
    public class GetAllCustomers
    {
        public record GetAllCustomersQuery()
            : IRequest<Result<List<GetAllCustomersResponse>>>;

        public class GetAllCustomersHandler(
            ICustomerRepository customerRepository)
            : IRequestHandler<GetAllCustomersQuery,
                Result<List<GetAllCustomersResponse>>>
        {
            public async Task<Result<List<GetAllCustomersResponse>>> Handle(
                GetAllCustomersQuery request,
                CancellationToken cancellationToken)
            {
                var customers = await customerRepository.GetCustomersAsync();

                var response = customers
                    .Where(c => !c.IsDeleted)
                    .Select(c => new GetAllCustomersResponse(
                        c.Id, c.Name, c.Email,
                        c.PhoneNo, c.Address,
                        c.Wallet?.Balance ?? 0,
                        c.Orders.Count,
                        c.DateCreated))
                    .ToList();

                return Result<List<GetAllCustomersResponse>>
                    .Success("Success", response);
            }
        }

        public record GetAllCustomersResponse(
            Guid CustomerId, string Name,
            string Email, string Phone,
            string Address, decimal WalletBalance,
            int TotalOrders, DateTime DateJoined);
    }
}