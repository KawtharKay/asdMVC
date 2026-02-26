using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository(AppDbContext context) : INotificationRepository
    {
        public async Task AddAsync(Notification notification)
        {
            await context.AddAsync(notification);
        }

        public async Task<ICollection<Notification>> GetAllAsync(Guid userId)
        {
            return await context.Notifications.Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
