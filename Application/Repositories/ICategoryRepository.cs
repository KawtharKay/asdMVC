using Domain.Entities;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(Category category);
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category?> GetCategoryAsync(string name);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        void Update(Category category);
    }
}
