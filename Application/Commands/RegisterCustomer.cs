using Application.Common;
using Application.Common.Dtos;
using Application.Constants;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands
{
    public class RegisterCustomer
    {
        public record RegisterCustomerCommand(string FullName, string Email, string Password, string ConfirmPassword, string Phone, string Address) : IRequest<Result<RegisterCustomerResponse>>;

        public class RegisterHandler(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IWalletRepository walletRepository,
            ICartRepository cartRepository,
            IEmailService emailService,
            IPasswordHasher<User> passwordHasher, 
            IUnitOfWork unitOfWork): IRequestHandler<RegisterCustomerCommand, Result<RegisterCustomerResponse>>
        {
            public async Task<Result<RegisterCustomerResponse>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
            {
                if (request.Password != request.ConfirmPassword)return Result<RegisterCustomerResponse>.Failure("Passwords do not match");

                var emailExists = await userRepository.GetAsync(request.Email);
                if (emailExists is not null) return Result<RegisterCustomerResponse>.Failure("Email already registered");

                var role = await roleRepository.GetAsync(AppRoles.Customer);
                if (role is null) return Result<RegisterCustomerResponse>.Failure("Default role not found");

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

                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };

                await userRoleRepository.AddAsync(userRole);

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

                var wallet = new Wallet
                {
                    CustomerId = customer.Id,
                    Balance = 0,
                    CreatedBy = request.Email
                };

                await walletRepository.AddWalletAsync(wallet);

                var cart = new Cart
                {
                    CustomerId = customer.Id,
                    CreatedBy = request.Email
                };

                await cartRepository.AddCartAsync(cart);
                await unitOfWork.SaveAsync();

                await emailService.SendEmailAsync(
                    user.Email,
                    "Verify Your Email",
                    EmailTemplates.VerificationEmail(
                        request.FullName, token));

                return Result<RegisterCustomerResponse>.Success(
                    "Registration successful! Please check your email for verification code.",
                    new RegisterCustomerResponse(user.Id, user.Email));
            }
        }

        public record RegisterCustomerResponse(Guid UserId, string Email);
    }
}