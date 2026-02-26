using Application;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToDbAsync(Product product)
        {
            await _context.Set<Product>().AddAsync(product);
        }

        public async Task<Product?> GetProductAsync(Guid id)
        {
            return await _context.Set<Product>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<Product>> GetProductsAsync()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        public async Task<bool> IsExistAsync(string sku)
        {
            return await _context.Set<Product>().AnyAsync(x => x.Sku == sku);
        }
    }
}
