using Application.Common;
using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using FluentValidation;
using MediatR;
using System.Transactions;

namespace Application.Commands
{
    public class VerifyFunding
    {
        public record VerifyFundingCommand(
            string Reference) : IRequest<Result<string>>;

        public class VerifyFundingValidator
            : AbstractValidator<VerifyFundingCommand>
        {
            public VerifyFundingValidator()
            {
                RuleFor(x => x.Reference)
                    .NotEmpty().WithMessage("Payment reference is required");
            }
        }

        public class VerifyFundingHandler(
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaystackService paystackService,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<VerifyFundingCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                VerifyFundingCommand request,
                CancellationToken cancellationToken)
            {
                var transaction = await walletTransactionRepository
                    .GetByReferenceAsync(request.Reference);

                if (transaction is null)
                    return Result<string>.Failure("Transaction not found");

                if (transaction.Status == WalletTransactionStatus.Success)
                    return Result<string>.Failure("Transaction already verified");

                var verification = await paystackService
                    .VerifyPaymentAsync(request.Reference);

                if (!verification.Status || verification.PaymentStatus != "success")
                {
                    transaction.Status = WalletTransactionStatus.Failed;
                    transaction.DateModified = DateTime.UtcNow;
                    await unitOfWork.SaveAsync();
                    return Result<string>.Failure("Payment verification failed");
                }

                // Credit wallet
                var wallet = await walletRepository
                    .GetByIdAsync(transaction.WalletId);

                transaction.BalanceBefore = wallet!.Balance;
                wallet.Balance += transaction.Amount;
                transaction.BalanceAfter = wallet.Balance;
                transaction.Status = WalletTransactionStatus.Success;
                transaction.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                // Send email notification
                await emailService.SendEmailAsync(
                    wallet.Customer.Email,
                    "Wallet Funded Successfully",
                    EmailTemplates.WalletFundedEmail(
                        wallet.Customer.Name,
                        transaction.Amount,
                        wallet.Balance));

                return Result<string>.Success(
                    "Wallet funded successfully!", "funded");
            }
        }
    }
}