using Domain.Entities;

namespace Application
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> IsExist(Guid id);
    }
}
