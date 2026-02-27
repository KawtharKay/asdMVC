using Application.Common;
using Application.Repositories;
using Mapster;
using MediatR;

namespace Application.Queries
{
    public class GetBankAccountByCustomerId
    {
        public record GetBankAccountByCustomerIdQuery(Guid CustomerId) : IRequest<Result<GetBankAccountByCustomerIdResponse>>;

        public class GetBankAccountByCustomerIdHandler : IRequestHandler<GetBankAccountByCustomerIdQuery, Result<GetBankAccountByCustomerIdResponse>>
        {
            private readonly IBankAccountRepository _bankAccountRepository;
            public GetBankAccountByCustomerIdHandler(IBankAccountRepository bankAccountRepository)
            {
                _bankAccountRepository = bankAccountRepository;
            }
            public async Task<Result<GetBankAccountByCustomerIdResponse>> Handle(GetBankAccountByCustomerIdQuery request, CancellationToken cancellationToken)
            {
                var bankAccount = await _bankAccountRepository.GetBankAccountAsync(request.CustomerId);
                if (bankAccount is null) throw new Exception("Bank account not found");

                return Result<GetBankAccountByCustomerIdResponse>.Success("Success!", bankAccount.Adapt<GetBankAccountByCustomerIdResponse>());
            }
        }

        public record GetBankAccountByCustomerIdResponse(Guid Id, Guid CustomerId, string BankName, string BankCode, string AccountNumber, string AccountName, bool IsDefault);
    }
}