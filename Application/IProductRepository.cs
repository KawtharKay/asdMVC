using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IProductRepository
    {
        Task<bool> IsExistAsync(string sku);
        Task AddToDbAsync(Product product);
        Task<Product?> GetProductAsync(Guid id);
        Task<ICollection<Product>> GetProductsAsync();
    }
}
