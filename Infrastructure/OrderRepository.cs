using Application;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddToDbAsync(Order order)
        {
            await _context.Set<Order>().AddAsync(order);
        }

        public async Task<Order?> GetOrderAsync(Guid id)
        {
            return await _context.Set<Order>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<Order>> GetOrdersAsync()
        {
            return await _context.Set<Order>().ToListAsync();
        }

        public async Task<bool> IsExistAsync(string orderNo)
        {
            return await _context.Set<Order>().AnyAsync(x => x.OrderNo == orderNo);
        }
    }
}
