using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRoleRepository(AppDbContext context) : IUserRoleRepository
    {
        public async Task AddAsync(UserRole userRole)
        {
            await context.AddAsync(userRole);
        }

        public async Task<bool> IsExist(Guid userId, Guid roleId)
        {
            return await context.UserRoles.AnyAsync(a => a.UserId == userId && a.RoleId == roleId);
        }
    }
}
