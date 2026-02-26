using Domain.Entities;

namespace Application.Repositories
{
    public interface IRoleRepository
    {
        Task<bool> IsExist(string name);
        Task AddAsync(Role role);
        Task<ICollection<Role>> GetAllAsync();
    }
}
