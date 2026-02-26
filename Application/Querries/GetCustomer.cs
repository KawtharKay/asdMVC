using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Querries
{
    public class GetCustomer
    {
        public record GetCustomerQuery(Guid Id) : IRequest<Result<GetCustomerResponse>>;
        public class GetCustomerHandler(ICustomerRepository customerRepository) : IRequestHandler<GetCustomerQuery, Result<GetCustomerResponse>>
        {
            public async Task<Result<GetCustomerResponse>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
            {
                var response = await customerRepository.GetCustomerAsync(request.Id);
                if (response is null) throw new Exception("Customer not found");

                return Result<GetCustomerResponse>.Success("Success!", response.Adapt<GetCustomerResponse>());
            }
        }

        public record GetCustomerResponse(Guid Id, string Email);
    }
}
