using Application.Common;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateBankAccount
    {
        public record CreateBankAccountCommand(Guid CustomerId, string BankName, string BankCode, string AccountNumber, string AccountName, string? RecipientCode) : IRequest<Result<CreateBankAccountResponse>>;
        public class CreateBankAccountHandler : IRequestHandler<CreateBankAccountCommand, Result<CreateBankAccountResponse>>
        {
            private readonly IBankAccountRepository _bankAccountRepository;
            private readonly IUnitOfWork _unitOfWork;
            public CreateBankAccountHandler(IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork)
            {
                _bankAccountRepository = bankAccountRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Result<CreateBankAccountResponse>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
            {
                var bankAccountExist = await _bankAccountRepository.GetAllBankAccountsByCustomerAsync(request.CustomerId);

                var bankAccount = new BankAccount
                {
                    CustomerId = request.CustomerId,
                    BankName = request.BankName,
                    BankCode = request.BankCode,
                    AccountNumber = request.AccountNumber,
                    AccountName = request.AccountName,
                    RecipientCode = request.RecipientCode
                };

                await _bankAccountRepository.AddAsync(bankAccount);
                await _unitOfWork.SaveAsync();

                return Result<CreateBankAccountResponse>.Success("Success!", bankAccount.Adapt<CreateBankAccountResponse>());
            }
        }

        public record CreateBankAccountResponse(Guid Id, Guid CustomerId, string BankName, string BankCode, string AccountNumber, string AccountName, string? RecipientCode);
    }
}