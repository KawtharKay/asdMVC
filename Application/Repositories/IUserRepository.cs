using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetAsync(Guid id);
        Task<User?> GetAsync(string email);
        Task<bool> IsExistAsync(string email);
        Task<ICollection<User>> GetAllAsync();
    }
}