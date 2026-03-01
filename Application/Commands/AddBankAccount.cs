using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class AddBankAccount
    {
        public record AddBankAccountCommand(
            Guid CustomerId,
            string BankName,
            string BankCode,
            string AccountNumber,
            string AccountName,
            bool IsDefault) : IRequest<Result<string>>;

        public class AddBankAccountValidator
            : AbstractValidator<AddBankAccountCommand>
        {
            public AddBankAccountValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");

                RuleFor(x => x.BankName)
                    .NotEmpty().WithMessage("Bank name is required")
                    .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters");

                RuleFor(x => x.BankCode)
                    .NotEmpty().WithMessage("Bank code is required");

                RuleFor(x => x.AccountNumber)
                    .NotEmpty().WithMessage("Account number is required")
                    .Length(10).WithMessage("Account number must be exactly 10 digits")
                    .Matches(@"^\d+$").WithMessage("Account number must contain only digits");

                RuleFor(x => x.AccountName)
                    .NotEmpty().WithMessage("Account name is required")
                    .MaximumLength(150).WithMessage("Account name cannot exceed 150 characters");
            }
        }

        public class AddBankAccountHandler(
            IBankAccountRepository bankAccountRepository,
            IPaystackService paystackService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddBankAccountCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(AddBankAccountCommand request,CancellationToken cancellationToken)
            {
                var verification = await paystackService.VerifyAccountNumberAsync( request.AccountNumber, request.BankCode);

                if (!verification.Status)return Result<string>.Failure("Account verification failed");

                var recipientCode = await paystackService .CreateTransferRecipientAsync(request.AccountName,request.AccountNumber, request.BankCode);

                if (request.IsDefault)
                {
                    var existing = await bankAccountRepository.GetByCustomerIdAsync(request.CustomerId);

                    foreach (var account in existing.Where(a => a.IsDefault))
                    {
                        account.IsDefault = false;
                        account.DateModified = DateTime.UtcNow;
                    }
                }

                await bankAccountRepository.AddAsync(new BankAccount
                {
                    CustomerId = request.CustomerId,
                    BankName = request.BankName,
                    BankCode = request.BankCode,
                    AccountNumber = request.AccountNumber,
                    AccountName = request.AccountName,
                    RecipientCode = recipientCode,
                    IsDefault = request.IsDefault,
                    CreatedBy = request.CustomerId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Bank account added successfully", "added");
            }
        }
    }
}