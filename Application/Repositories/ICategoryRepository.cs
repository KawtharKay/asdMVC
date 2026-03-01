using Domain.Entities;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task<Category?> GetByIdAsync(Guid id);
        Task<bool> IsExistAsync(string name);
        Task<ICollection<Category>> GetAllAsync();
    }
}