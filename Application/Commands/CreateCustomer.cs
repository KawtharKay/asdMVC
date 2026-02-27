using Application.Common;
using Application.Constants;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Commands
{
    public class CreateCustomer
    {
        public record CreateCustomerCommand(Guid UserId, string Email, string Name, string Address, string PhoneNo) : IRequest<Result<CreateCustomerResponse>>;
        public class CreateCustomerHandler(ICustomerRepository customerRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResponse>>
        {
            public async Task<Result<CreateCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var role = await roleRepository.GetAsync(AppRoles.Customer);
                if (role is null) throw new Exception("Failure");

                var isRoleAssigned = await userRoleRepository.IsExist(request.UserId, role.Id);
                if (isRoleAssigned) throw new Exception("Role already assigned");

                var userRole = new UserRole
                {
                    UserId = request.UserId,
                    RoleId = role.Id
                };
                await userRoleRepository.AddAsync(userRole);

                Customer customer = new()
                {
                    UserId = request.UserId,
                    Email = request.Email,
                    Name = request.Name,
                    Address = request.Address,
                    PhoneNo = request.PhoneNo
                };
                await customerRepository.AddAsync(customer);
                await unitOfWork.SaveAsync();
                return Result<CreateCustomerResponse>.Success("Success!", customer.Adapt<CreateCustomerResponse>());
            }
        }

        public record CreateCustomerResponse(Guid Id);
    }
}
