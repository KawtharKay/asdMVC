using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);

        public async Task<Category?> GetByIdAsync(Guid id)
            => await context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        public async Task<bool> IsExistAsync(string name)
            => await context.Categories
                .AnyAsync(c => c.Name == name && !c.IsDeleted);

        public async Task<ICollection<Category>> GetAllAsync()
            => await context.Categories
                .Include(c => c.Products)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
    }
}