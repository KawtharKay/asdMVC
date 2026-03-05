using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        public async Task AddAsync(Notification notification)
            => await context.Notifications.AddAsync(notification);

        public async Task<Notification?> GetByIdAsync(Guid id)
            => await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

        public async Task<ICollection<Notification>> GetByUserIdAsync(
            Guid userId, bool unreadOnly = false)
        {
            var query = context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            return await query
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
            => await context.Notifications
                .CountAsync(n => n.UserId == userId
                    && !n.IsRead
                    && !n.IsDeleted);
    }
}