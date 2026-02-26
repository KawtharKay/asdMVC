using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IConversationRepository
    {
        Task AddToDbAsync(Conversation conversation);
        Task<Conversation?> GetConversationAsync(Guid id);
        Task<IEnumerable<Conversation>> GetAllConversationsAsync();
    }
}
