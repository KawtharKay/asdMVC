using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetTransactionHistory
    {
        public record GetTransactionHistoryQuery(Guid CustomerId)
            : IRequest<Result<List<GetTransactionHistoryResponse>>>;

        public class GetTransactionHistoryValidator
            : AbstractValidator<GetTransactionHistoryQuery>
        {
            public GetTransactionHistoryValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetTransactionHistoryHandler(
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository)
            : IRequestHandler<GetTransactionHistoryQuery,
                Result<List<GetTransactionHistoryResponse>>>
        {
            public async Task<Result<List<GetTransactionHistoryResponse>>> Handle(
                GetTransactionHistoryQuery request,
                CancellationToken cancellationToken)
            {
                var wallet = await walletRepository
                    .GetByCustomerIdAsync(request.CustomerId);
                if (wallet is null)
                    return Result<List<GetTransactionHistoryResponse>>
                        .Failure("Wallet not found");

                var transactions = await walletTransactionRepository
                    .GetByWalletIdAsync(wallet.Id);

                var response = transactions.Select(t =>
                    new GetTransactionHistoryResponse(
                        t.Id, t.Amount, t.Type.ToString(),
                        t.Status.ToString(), t.Description,
                        t.BalanceBefore, t.BalanceAfter,
                        t.DateCreated))
                    .ToList();

                return Result<List<GetTransactionHistoryResponse>>
                    .Success("Success", response);
            }
        }

        public record GetTransactionHistoryResponse(
            Guid TransactionId, decimal Amount,
            string Type, string Status, string Description,
            decimal BalanceBefore, decimal BalanceAfter,
            DateTime DateCreated);
    }
}