using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Querries
{
    public class GetAllCustomers
    {
        public record GetAllCustomerQuery() : IRequest<Result<ICollection<GetAllCustomerResponse>>>;
        public class GetAllCustomerHandler(ICustomerRepository customerRepository) : IRequestHandler<GetAllCustomerQuery, Result<ICollection<GetAllCustomerResponse>>>
        {
            public async Task<Result<ICollection<GetAllCustomerResponse>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
            {
                var response = await customerRepository.GetCustomersAsync();
                return Result<ICollection<GetAllCustomerResponse>>.Success("Success!", response.Adapt<ICollection<GetAllCustomerResponse>>());
            }
        }

        public record GetAllCustomerResponse(Guid Id, string Email);
    }
}
