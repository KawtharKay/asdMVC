using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Queries
{
    public class GetWalletBalance
    {
        public record GetWalletBalanceQuery(Guid CustomerId)
            : IRequest<Result<GetWalletBalanceResponse>>;

        public class GetWalletBalanceValidator
            : AbstractValidator<GetWalletBalanceQuery>
        {
            public GetWalletBalanceValidator()
            {
                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class GetWalletBalanceHandler(IWalletRepository walletRepository)
            : IRequestHandler<GetWalletBalanceQuery,
                Result<GetWalletBalanceResponse>>
        {
            public async Task<Result<GetWalletBalanceResponse>> Handle(
                GetWalletBalanceQuery request,
                CancellationToken cancellationToken)
            {
                var wallet = await walletRepository
                    .GetByCustomerIdAsync(request.CustomerId);
                if (wallet is null)
                    return Result<GetWalletBalanceResponse>.Failure("Wallet not found");

                return Result<GetWalletBalanceResponse>.Success(
                    "Success",
                    new GetWalletBalanceResponse(wallet.Id, wallet.Balance));
            }
        }

        public record GetWalletBalanceResponse(Guid WalletId, decimal Balance);
    }
}