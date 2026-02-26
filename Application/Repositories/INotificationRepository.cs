using Domain.Entities;

namespace Application.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<ICollection<Notification>> GetAllAsync(Guid userId);
    }
}
