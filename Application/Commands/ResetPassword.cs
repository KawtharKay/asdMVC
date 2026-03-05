using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Repositories;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands
{
    public class ResetPassword
    {
        public record ResetPasswordCommand(
            string Email,
            string Token,
            string NewPassword,
            string ConfirmPassword)
            : IRequest<Result<string>>, ISensitiveRequest;

        public class ResetPasswordValidator
            : AbstractValidator<ResetPasswordCommand>
        {
            public ResetPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Invalid email format");

                RuleFor(x => x.Token)
                    .NotEmpty().WithMessage("Reset code is required")
                    .Length(4).WithMessage("Reset code must be 4 digits")
                    .Matches(@"^\d{4}$").WithMessage("Reset code must contain only digits");

                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("New password is required")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
            }
        }

        public class ResetPasswordHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IUnitOfWork unitOfWork)
            : IRequestHandler<ResetPasswordCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                ResetPasswordCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.Email);
                if (user is null)
                    return Result<string>.Failure("Invalid request");

                if (user.VerificationToken != request.Token)
                    return Result<string>.Failure("Invalid reset code");

                if (user.VerificationTokenExpiry < DateTime.UtcNow)
                    return Result<string>.Failure(
                        "Reset code has expired. Please request a new one");

                user.HashPassword = passwordHasher.HashPassword(user, request.NewPassword);
                user.VerificationToken = null;
                user.VerificationTokenExpiry = null;
                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(
                    "Password reset successfully! You can now login.", "reset");
            }
        }
    }
}