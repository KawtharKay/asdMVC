using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task<bool> IsExist(Guid userId, Guid roleId);
    }
}
