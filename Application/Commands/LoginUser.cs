using Application.Common.Dtos;
using Application.Constants;
using Application.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace Application.Commands
{
    public class LoginUser
    {
        public record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginUserResponse>>;

        public class LoginUserHandler(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IPasswordHasher<User> passwordHasher) 
            : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
        {
            public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.Email);
                if (user is null) return Result<LoginUserResponse>.Failure("Invalid email or password");

                if (!user.IsVerified) return Result<LoginUserResponse>.Failure("Please verify your email before logging in");

                var result = passwordHasher.VerifyHashedPassword(user, user.HashPassword, request.Password);
                if (result == PasswordVerificationResult.Failed)
                    return Result<LoginUserResponse>.Failure("Invalid email or password");

                var customer = await customerRepository.GetCustomerAsync(user.Id);
                if (customer is null)
                    return Result<LoginUserResponse>.Failure("Customer profile not found");

                var roles = await roleRepository.GetAllAsync();
                var userRoles = roles.Where(r => r.UserRoles.Any(ur => ur.UserId == user.Id)).Select(r => r.Name).ToList();
                var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Customer;

                return Result<LoginUserResponse>.Success(
                    "Login successful", new LoginUserResponse(user.Id, customer.Id, user.Fullname ?? "User", user.Email, primaryRole));
            }
        }

        public record LoginUserResponse(Guid UserId, Guid CustomerId, string FullName, string Email, string Role);
    }
}