using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetCustomerById
    {
        public record GetCustomerByIdQuery(Guid CustomerId)
            : IRequest<Result<GetCustomerByIdResponse>>;

        public class GetCustomerByIdValidator
            : AbstractValidator<GetCustomerByIdQuery>
        {
            public GetCustomerByIdValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetCustomerByIdHandler(
            ICustomerRepository customerRepository)
            : IRequestHandler<GetCustomerByIdQuery,
                Result<GetCustomerByIdResponse>>
        {
            public async Task<Result<GetCustomerByIdResponse>> Handle(
                GetCustomerByIdQuery request,
                CancellationToken cancellationToken)
            {
                var c = await customerRepository
                    .GetCustomerAsync(request.CustomerId);

                if (c is null)
                    return Result<GetCustomerByIdResponse>
                        .Failure("Customer not found");

                return Result<GetCustomerByIdResponse>.Success(
                    "Success",
                    new GetCustomerByIdResponse(
                        c.Id, c.Name, c.Email,
                        c.PhoneNo, c.Address,
                        c.Wallet?.Balance ?? 0,
                        c.Orders.Count,
                        c.DateCreated));
            }
        }

        public record GetCustomerByIdResponse(
            Guid CustomerId, string Name,
            string Email, string Phone,
            string Address, decimal WalletBalance,
            int TotalOrders, DateTime DateJoined);
    }
}