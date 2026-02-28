using Application.Common.Dtos;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetAllBankAccounts
    {
        public record GetAllBankAccountsQuery() : IRequest<Result<IEnumerable<GetAllBankAccountsResponse>>>;

        public class GetAllBankAccountsHandler : IRequestHandler<GetAllBankAccountsQuery, Result<IEnumerable<GetAllBankAccountsResponse>>>
        {
            private readonly IBankAccountRepository _bankAccountRepository;
            public GetAllBankAccountsHandler(IBankAccountRepository bankAccountRepository)
            {
                _bankAccountRepository = bankAccountRepository;
            }
            public async Task<Result<IEnumerable<GetAllBankAccountsResponse>>> Handle(GetAllBankAccountsQuery request, CancellationToken cancellationToken)
            {
                var response = await _bankAccountRepository.GetAllBankAccountsAsync();
                return Result<IEnumerable<GetAllBankAccountsResponse>>.Success("Success!", response.Adapt<IEnumerable<GetAllBankAccountsResponse>>());
            }
        }

        public record GetAllBankAccountsResponse(Guid Id, Guid CustomerId, string BankName, string BankCode, string AccountNumber, string AccountName, bool IsDefault);
    }
}