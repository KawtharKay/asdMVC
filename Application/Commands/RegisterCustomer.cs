using Application.Common;
using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Constants;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands
{
    public class RegisterCustomer
    {
        public record RegisterCustomerCommand(
            string FullName,
            string Email,
            string Password,
            string ConfirmPassword,
            string Phone,
            string Address)
            : IRequest<Result<RegisterResponse>>, ISensitiveRequest;

        public class RegisterCustomerValidator : AbstractValidator<RegisterCustomerCommand>
        {
            public RegisterCustomerValidator()
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage("Full name is required")
                    .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required")
                    .EmailAddress().WithMessage("Invalid email format");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.Password).WithMessage("Passwords do not match");

                RuleFor(x => x.Phone)
                    .NotEmpty().WithMessage("Phone number is required")
                    .Matches(@"^\d{11}$").WithMessage("Phone must be 11 digits");

                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Address is required")
                    .MaximumLength(300).WithMessage("Address cannot exceed 300 characters");
            }
        }

        public class RegisterCustomerHandler(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IWalletRepository walletRepository,
            ICartRepository cartRepository,
            IEmailService emailService,
            IPasswordHasher<User> passwordHasher,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RegisterCustomerCommand, Result<RegisterResponse>>
        {
            public async Task<Result<RegisterResponse>> Handle(
                RegisterCustomerCommand request,
                CancellationToken cancellationToken)
            {
                var emailExists = await userRepository.GetAsync(request.Email);
                if (emailExists is not null)
                    return Result<RegisterResponse>.Failure("Email already registered");

                var role = await roleRepository.GetAsync(AppRoles.Customer);
                if (role is null)
                    return Result<RegisterResponse>.Failure("Default role not found");

                var token = new Random().Next(1000, 9999).ToString();

                var user = new User
                {
                    Fullname = request.FullName,
                    Email = request.Email,
                    IsVerified = false,
                    VerificationToken = token,
                    VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5),
                    CreatedBy = request.Email
                };

                user.HashPassword = passwordHasher.HashPassword(user, request.Password);

                await userRepository.AddAsync(user);

                await userRoleRepository.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });

                var customer = new Customer
                {
                    UserId = user.Id,
                    Name = request.FullName,
                    Email = request.Email,
                    PhoneNo = request.Phone,
                    Address = request.Address,
                    CreatedBy = request.Email
                };

                await customerRepository.AddAsync(customer);

                await walletRepository.AddWalletAsync(new Wallet
                {
                    CustomerId = customer.Id,
                    Balance = 0,
                    CreatedBy = request.Email
                });

                await cartRepository.AddCartAsync(new Cart
                {
                    CustomerId = customer.Id,
                    CreatedBy = request.Email
                });

                await unitOfWork.SaveAsync();

                await emailService.SendEmailAsync(
                    user.Email,
                    "Verify Your Email",
                    EmailTemplates.VerificationEmail(request.FullName, token));

                return Result<RegisterResponse>.Success(
                    "Registration successful! Please check your email for verification code.",
                    new RegisterResponse(user.Id, user.Email));
            }
        }

        public record RegisterResponse(Guid UserId, string Email);
    }
}