using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetMyBankAccounts
    {
        public record GetMyBankAccountsQuery(Guid CustomerId)
            : IRequest<Result<List<GetMyBankAccountsResponse>>>;

        public class GetMyBankAccountsValidator
            : AbstractValidator<GetMyBankAccountsQuery>
        {
            public GetMyBankAccountsValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetMyBankAccountsHandler(
            IBankAccountRepository bankAccountRepository)
            : IRequestHandler<GetMyBankAccountsQuery,
                Result<List<GetMyBankAccountsResponse>>>
        {
            public async Task<Result<List<GetMyBankAccountsResponse>>> Handle(
                GetMyBankAccountsQuery request,
                CancellationToken cancellationToken)
            {
                var accounts = await bankAccountRepository
                    .GetByCustomerIdAsync(request.CustomerId);

                var response = accounts
                    .Where(a => !a.IsDeleted)
                    .Select(a => new GetMyBankAccountsResponse(
                        a.Id, a.BankName,
                        a.AccountNumber,
                        a.AccountName, a.IsDefault))
                    .ToList();

                return Result<List<GetMyBankAccountsResponse>>
                    .Success("Success", response);
            }
        }

        public record GetMyBankAccountsResponse(
            Guid Id, string BankName,
            string AccountNumber,
            string AccountName, bool IsDefault);
    }
}