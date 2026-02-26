using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Repositories
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public async Task AddAsync(Message message)
        {
            await context.AddAsync(message);
        }

        public async Task<ICollection<Message>> GetAllAsync(Guid senderId)
        {
            return await context.Messages.Where(a => a.SenderId == senderId).ToListAsync();
        }
    }
}
