using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        public async Task<bool> IsExist(string name)
        {
            return await context.Roles.AnyAsync(r => r.Name == name);
        }

        public async Task AddAsync(Role role)
        {
            await context.Roles.AddAsync(role);
        }

        public async Task<ICollection<Role>> GetAllAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task<Role?> GetAsync(Guid id)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Role?> GetAsync(string name)
        {
            return await context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
