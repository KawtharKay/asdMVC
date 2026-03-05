using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task AddAsync(User user)
            => await context.Users.AddAsync(user);

        public async Task<User?> GetAsync(Guid id)
            => await context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        public async Task<User?> GetAsync(string email)
            => await context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

        public async Task<bool> IsExistAsync(string email)
            => await context.Users
                .AnyAsync(u => u.Email == email && !u.IsDeleted);

        public async Task<ICollection<User>> GetAllAsync()
            => await context.Users
                .Include(u => u.Customer)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
    }
}