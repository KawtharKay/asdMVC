using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> IsExist(Guid id);
        Task<User?> GetAsync(string email);
        Task<User?> GetAsync(Guid id);
    }
}
