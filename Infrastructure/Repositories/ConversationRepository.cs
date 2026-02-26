using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;
        public ConversationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToDbAsync(Conversation conversation)
        {
            await _context.Set<Conversation>().AddAsync(conversation);
        }

        public async Task<IEnumerable<Conversation>> GetAllConversationsAsync()
        {
            return await _context.Set<Conversation>().ToListAsync();
        }

        public async Task<Conversation?> GetConversationAsync(Guid id)
        {
            return await _context.Set<Conversation>().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
