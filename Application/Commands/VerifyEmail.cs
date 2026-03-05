using Application.Common;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Repositories;
using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class VerifyEmail
    {
        public record VerifyEmailCommand(
            string Email,
            string Token)
            : IRequest<Result<string>>, ISensitiveRequest;

        public class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
        {
            public VerifyEmailValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Invalid email format");

                RuleFor(x => x.Token)
                    .NotEmpty().WithMessage("Verification code is required")
                    .Length(4).WithMessage("Verification code must be 4 digits")
                    .Matches(@"^\d{4}$").WithMessage("Verification code must contain only digits");
            }
        }

        public class VerifyEmailHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<VerifyEmailCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                VerifyEmailCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.Email);
                if (user is null)
                    return Result<string>.Failure("User not found");

                if (user.IsVerified)
                    return Result<string>.Failure("Email already verified");

                if (user.VerificationToken != request.Token)
                    return Result<string>.Failure("Invalid verification code");

                if (user.VerificationTokenExpiry < DateTime.UtcNow)
                    return Result<string>.Failure(
                        "Verification code has expired. Please request a new one");

                user.IsVerified = true;
                user.VerificationToken = null;
                user.VerificationTokenExpiry = null;
                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                await emailService.SendEmailAsync(
                    user.Email,
                    "Welcome to EcommerceApp 🎉",
                    EmailTemplates.WelcomeEmail(user.Fullname ?? "Customer"));

                return Result<string>.Success(
                    "Email verified successfully! You can now login.", "verified");
            }
        }
    }
}