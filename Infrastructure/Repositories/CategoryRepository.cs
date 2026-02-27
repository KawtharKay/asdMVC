using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCategoryAsync(Category category)
        {
            await _context.Set<Category>().AddAsync(category);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Set<Category>().ToListAsync();
        }

        public async Task<Category?> GetCategoryAsync(string name)
        {
            return await _context.Set<Category>().FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Set<Category>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(Category category)
        {
            _context.Set<Category>().Update(category);
        }
    }
}
