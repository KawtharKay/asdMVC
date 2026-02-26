using Application;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
        }

        public async Task<bool> IsExist(Guid id)
        {
            return await context.Users.AnyAsync(u => u.Id == id);
        }
    }
}
