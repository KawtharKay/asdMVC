using Domain.Entities;

namespace Application
{
    public interface IRoleRepository
    {
        Task<bool> IsExist(string name);
        Task AddAsync(Role role);
        Task<ICollection<Role>> GetAllAsync();
    }
}
