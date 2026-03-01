using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        public async Task AddToDbAsync(Product product)
            => await context.Products.AddAsync(product);

        public async Task<Product?> GetByIdAsync(Guid id)
            => await context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        public async Task<bool> IsExistAsync(string sku)
            => await context.Products
                .AnyAsync(p => p.Sku == sku && !p.IsDeleted);

        public async Task<ICollection<Product>> GetAllAsync()
            => await context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted)
                .ToListAsync();

        public async Task<ICollection<Product>> GetByCategoryIdAsync(Guid categoryId)
            => await context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
    }
}