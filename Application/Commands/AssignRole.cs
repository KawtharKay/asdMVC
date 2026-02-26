using Application.Common;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class AssignRole
    {
        public record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<Result<AssignRoleResponse>>;
        public class AssignRoleHandler(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork) : IRequestHandler<AssignRoleCommand, Result<AssignRoleResponse>>
        {
            public async Task<Result<AssignRoleResponse>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
            {
                var user = await userRepository.GetAsync(request.UserId);
                if (user is null) throw new Exception("User does not exist");

                var role = await roleRepository.GetAsync(request.RoleId);
                if (role is null) throw new Exception("User does not exist");

                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = request.RoleId,
                };

                await userRoleRepository.AddAsync(userRole);
                await unitOfWork.SaveAsync();
                return Result<AssignRoleResponse>.Success("Success!", userRole.Adapt<AssignRoleResponse>());
            }
        }

        public record AssignRoleResponse(Guid UserId);
    }
}
