using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        public async Task AddAsync(Order order)
            => await context.Orders.AddAsync(order);

        public async Task<Order?> GetByIdAsync(Guid id)
            => await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.Product)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);

        public async Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId)
            => await context.Orders
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.Product)
                .Where(o => o.CustomerId == customerId && !o.IsDeleted)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();

        public async Task<ICollection<Order>> GetAllAsync()
            => await context.Orders
                .Include(o => o.Customer)
                .Include(o => o.ProductOrders)
                    .ThenInclude(po => po.Product)
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.DateCreated)
                .ToListAsync();
    }
}