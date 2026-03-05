using Domain.Entities;

namespace Application.Repositories
{
    public interface IProductRepository
    {
        Task AddToDbAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<bool> IsExistAsync(string sku);
        Task<ICollection<Product>> GetAllAsync();
        Task<ICollection<Product>> GetByCategoryIdAsync(Guid categoryId);
    }
}