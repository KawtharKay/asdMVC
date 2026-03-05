using Application.Common;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Repositories;
using Application.Services;
using FluentValidation;
using MediatR;

namespace Application.Commands
{
    public class ForgotPassword
    {
        public record ForgotPasswordCommand(
            string Email)
            : IRequest<Result<string>>, ISensitiveRequest;

        public class ForgotPasswordValidator
            : AbstractValidator<ForgotPasswordCommand>
        {
            public ForgotPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Invalid email format");
            }
        }

        public class ForgotPasswordHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<ForgotPasswordCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                ForgotPasswordCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.Email);

                if (user is null)
                    return Result<string>.Success(
                        "If this email exists you will receive a reset code shortly", "sent");

                var token = new Random().Next(1000, 9999).ToString();

                user.VerificationToken = token;
                user.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5);
                user.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                await emailService.SendEmailAsync(user.Email,"Password Reset Code",EmailTemplates.ForgotPasswordEmail(user.Fullname ?? "", token));

                return Result<string>.Success("If this email exists you will receive a reset code shortly", "sent");
            }
        }
    }
}