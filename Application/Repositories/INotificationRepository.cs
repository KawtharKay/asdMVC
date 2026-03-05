using Domain.Entities;

namespace Application.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<ICollection<Notification>> GetByUserIdAsync(Guid userId, bool unreadOnly = false);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}