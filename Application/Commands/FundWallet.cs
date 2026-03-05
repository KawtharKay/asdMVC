using Application.Common.Dtos;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class FundWallet
    {
        public record FundWalletCommand(
            Guid CustomerId,
            decimal Amount) : IRequest<Result<FundWalletResponse>>;

        public class FundWalletValidator : AbstractValidator<FundWalletCommand>
        {
            public FundWalletValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");

                RuleFor(x => x.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than zero");
            }
        }

        public class FundWalletHandler(
            ICustomerRepository customerRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaystackService paystackService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<FundWalletCommand, Result<FundWalletResponse>>
        {
            public async Task<Result<FundWalletResponse>> Handle(
                FundWalletCommand request,
                CancellationToken cancellationToken)
            {
                var customer = await customerRepository
                    .GetCustomerAsync(request.CustomerId);
                if (customer is null)
                    return Result<FundWalletResponse>.Failure("Customer not found");

                var wallet = await walletRepository
                    .GetByCustomerIdAsync(request.CustomerId);
                if (wallet is null)
                    return Result<FundWalletResponse>.Failure("Wallet not found");

                var reference = $"FUND-{Guid.NewGuid():N}";

                var paystackResponse = await paystackService.InitializePaymentAsync(
                    customer.Email, request.Amount, reference);

                if (!paystackResponse.Status)
                    return Result<FundWalletResponse>.Failure(
                        "Could not initialize payment");

                await walletTransactionRepository.AddAsync(new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = request.Amount,
                    Type = TransactionType.Credit,
                    Status = WalletTransactionStatus.Pending,
                    PaystackReference = reference,
                    Description = "Wallet Funding",
                    BalanceBefore = wallet.Balance,
                    BalanceAfter = wallet.Balance,
                    CreatedBy = customer.Email
                });

                await unitOfWork.SaveAsync();

                return Result<FundWalletResponse>.Success(
                    "Payment initialized",
                    new FundWalletResponse(
                        paystackResponse.AuthorizationUrl, reference));
            }
        }

        public record FundWalletResponse(
            string AuthorizationUrl, string Reference);
    }
}