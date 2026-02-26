using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<ICollection<Message>> GetAllAsync(Guid senderId);
    }
}
