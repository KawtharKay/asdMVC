using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class DeleteBankAccount
    {
        public record DeleteBankAccountCommand(
            Guid BankAccountId,
            Guid CustomerId) : IRequest<Result<string>>;

        public class DeleteBankAccountValidator
            : AbstractValidator<DeleteBankAccountCommand>
        {
            public DeleteBankAccountValidator()
            {
                RuleFor(x => x.BankAccountId)
                    .NotEmpty().WithMessage("Bank account ID is required");

                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class DeleteBankAccountHandler(
            IBankAccountRepository bankAccountRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<DeleteBankAccountCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                DeleteBankAccountCommand request,
                CancellationToken cancellationToken)
            {
                var account = await bankAccountRepository
                    .GetByIdAsync(request.BankAccountId);

                if (account is null)
                    return Result<string>.Failure("Bank account not found");

                if (account.CustomerId != request.CustomerId)
                    return Result<string>.Failure("Unauthorized");

                account.IsDeleted = true;
                account.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Bank account deleted successfully!", "deleted");
            }
        }
    }
}