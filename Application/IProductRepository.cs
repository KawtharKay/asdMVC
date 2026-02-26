using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IProductRepository
    {
        Task AddToDbAsync(Product product);
        Task<Product>? GetProductAsync(Guid id);
        Task<ICollection<Product>> GetProductsAsync();
    }
}
