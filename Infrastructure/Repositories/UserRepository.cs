using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
        }

        public async Task<User?> GetAsync(string email)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await context.Users.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> IsExist(Guid id)
        {
            return await context.Users.AnyAsync(u => u.Id == id);
        }
    }
}
