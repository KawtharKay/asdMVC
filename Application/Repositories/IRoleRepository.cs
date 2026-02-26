using Domain.Entities;

namespace Application.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> IsExist(string name);
        Task AddAsync(Role role);
        Task<Role?> GetAsync(Guid id);
        Task<Role?> GetAsync(string name);
        Task<ICollection<Role>> GetAllAsync();
    }
}
