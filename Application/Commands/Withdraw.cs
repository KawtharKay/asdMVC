using Application.Common;
using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using System.Transactions;

namespace Application.Commands
{
    public class Withdraw
    {
        public record WithdrawCommand(
            Guid CustomerId,
            decimal Amount,
            string AccountNumber,
            string BankCode,
            string BankName,
            string AccountName,
            Guid? SavedBankAccountId,
            bool SaveAccount) : IRequest<Result<string>>;

        public class WithdrawValidator : AbstractValidator<WithdrawCommand>
        {
            public WithdrawValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");

                RuleFor(x => x.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than zero");

                // Only validate account fields if not using a saved account
                When(x => !x.SavedBankAccountId.HasValue, () =>
                {
                    RuleFor(x => x.AccountNumber)
                        .NotEmpty().WithMessage("Account number is required")
                        .Length(10).WithMessage("Account number must be 10 digits");

                    RuleFor(x => x.BankCode)
                        .NotEmpty().WithMessage("Bank code is required");

                    RuleFor(x => x.BankName)
                        .NotEmpty().WithMessage("Bank name is required");

                    RuleFor(x => x.AccountName)
                        .NotEmpty().WithMessage("Account name is required");
                });
            }
        }

        public class WithdrawHandler(
            ICustomerRepository customerRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IBankAccountRepository bankAccountRepository,
            IPaystackService paystackService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<WithdrawCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                WithdrawCommand request,
                CancellationToken cancellationToken)
            {
                var customer = await customerRepository
                    .GetCustomerAsync(request.CustomerId);
                if (customer is null)
                    return Result<string>.Failure("Customer not found");

                var wallet = await walletRepository
                    .GetByCustomerIdAsync(request.CustomerId);
                if (wallet is null)
                    return Result<string>.Failure("Wallet not found");

                if (wallet.Balance < request.Amount)
                    return Result<string>.Failure("Insufficient wallet balance");

                string recipientCode;
                string accountName;
                string accountNumber;
                string bankName;

                if (request.SavedBankAccountId.HasValue)
                {
                    var saved = await bankAccountRepository
                        .GetByIdAsync(request.SavedBankAccountId.Value);

                    if (saved is null)
                        return Result<string>.Failure("Saved bank account not found");

                    accountName = saved.AccountName;
                    accountNumber = saved.AccountNumber;
                    bankName = saved.BankName;

                    if (string.IsNullOrEmpty(saved.RecipientCode))
                    {
                        saved.RecipientCode = await paystackService
                            .CreateTransferRecipientAsync(
                                saved.AccountName,
                                saved.AccountNumber,
                                saved.BankCode);
                        await unitOfWork.SaveAsync();
                    }

                    recipientCode = saved.RecipientCode!;
                }
                else
                {
                    accountName = request.AccountName;
                    accountNumber = request.AccountNumber;
                    bankName = request.BankName;

                    recipientCode = await paystackService
                        .CreateTransferRecipientAsync(
                            accountName, accountNumber, request.BankCode);

                    if (request.SaveAccount)
                    {
                        var existing = await bankAccountRepository
                            .GetByCustomerIdAsync(request.CustomerId);

                        await bankAccountRepository.AddAsync(new BankAccount
                        {
                            CustomerId = customer.Id,
                            BankName = bankName,
                            BankCode = request.BankCode,
                            AccountNumber = accountNumber,
                            AccountName = accountName,
                            RecipientCode = recipientCode,
                            IsDefault = !existing.Any(),
                            CreatedBy = customer.Email
                        });
                    }
                }

                var reference = $"WDR-{Guid.NewGuid():N}";

                var transfer = await paystackService.InitiateTransferAsync(
                    recipientCode, request.Amount, reference,
                    $"Withdrawal by {customer.Email}");

                if (!transfer.Status)
                    return Result<string>.Failure("Transfer initiation failed");

                var balanceBefore = wallet.Balance;
                wallet.Balance -= request.Amount;

                await walletTransactionRepository.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = request.Amount,
                    Type = TransactionType.Withdrawal,
                    Status = WalletTransactionStatus.Pending,
                    PaystackReference = reference,
                    Description = $"Withdrawal to {accountNumber} ({bankName})",
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    CreatedBy = customer.Email
                });

                await unitOfWork.SaveAsync();

                await emailService.SendEmailAsync(
                    customer.Email,
                    "Withdrawal Initiated",
                    EmailTemplates.WithdrawalEmail(
                        customer.Name, request.Amount,
                        accountNumber, bankName, wallet.Balance));

                return Result<string>.Success(
                    "Withdrawal initiated successfully!", "pending");
            }
        }
    }
}