//using Application.Repositories;
//using MediatR;
//using Microsoft.AspNetCore.Identity;

//namespace Application.Commands
//{
//    public class LoginUser
//    {
//        public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;
//        public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResponse>
//        {
//            private readonly IUnitOfWork _unitOfWork;
//            private readonly IUserRepository _userRepository;
//            private readonly IPasswordHasher<string> _passwordHasher;
//            private readonly ITokenServices _tokenServices;

//            public LoginUserHandler(IUnitOfWork unitOfWork, IUserRepository user, IPasswordHasher<string> hasher, ICurrentUser currentUser, ITokenServices tokenServices)
//            {
//                _unitOfWork = unitOfWork;
//                _userRepository = user;
//                _passwordHasher = hasher;
//                _tokenServices = tokenServices;
//            }

//            public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
//            {
//                var user = await _userRepository.GetAsync(request.Email);
//                if (user is null)
//                {
//                    throw new Exception("Invalid Credential");
//                }
//                string password = $"{request.Password}{user.Salt}";
//                var verificationResult = _passwordHasher.VerifyHashedPassword(request.Email, user.HashPassword, password);

//                if (verificationResult == PasswordVerificationResult.Failed)
//                {
//                    throw new Exception("Invalid credential");
//                }
//                var token = _tokenServices.GenerateToken(new LoginResponse
//                {
//                    Id = user.Id,
//                    Email = user.Email,
//                    Role = user.Role,
//                });
//                return new LoginUserResponse(token);
//            }
//        }
//        public record LoginUserResponse(string token);
//    }
//}
