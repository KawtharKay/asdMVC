using Application.Common.Dtos;
using Application.Repositories;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class SetDefaultBankAccount
    {
        public record SetDefaultBankAccountCommand(
            Guid BankAccountId,
            Guid CustomerId) : IRequest<Result<string>>;

        public class SetDefaultBankAccountValidator
            : AbstractValidator<SetDefaultBankAccountCommand>
        {
            public SetDefaultBankAccountValidator()
            {
                RuleFor(x => x.BankAccountId)
                    .NotEmpty().WithMessage("Bank account ID is required");

                RuleFor(x => x.CustomerId)
                    .NotEmpty().WithMessage("Customer ID is required");
            }
        }

        public class SetDefaultBankAccountHandler(
            IBankAccountRepository bankAccountRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<SetDefaultBankAccountCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                SetDefaultBankAccountCommand request,
                CancellationToken cancellationToken)
            {
                var accounts = await bankAccountRepository
                    .GetByCustomerIdAsync(request.CustomerId);

                var target = accounts
                    .FirstOrDefault(a => a.Id == request.BankAccountId);

                if (target is null)
                    return Result<string>.Failure("Bank account not found");

                // Unset previous default
                foreach (var account in accounts.Where(a => a.IsDefault))
                {
                    account.IsDefault = false;
                    account.DateModified = DateTime.UtcNow;
                }

                // Set new default
                target.IsDefault = true;
                target.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Default bank account updated!", "updated");
            }
        }
    }
}