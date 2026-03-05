using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository(AppDbContext context) : ICustomerRepository
    {
        public async Task AddAsync(Customer customer)
            => await context.Customers.AddAsync(customer);

        public async Task<Customer?> GetCustomerAsync(Guid id)
            => await context.Customers
                .Include(c => c.User)
                .Include(c => c.Wallet)
                .Include(c => c.Cart)
                    .ThenInclude(cart => cart.CartItems)
                        .ThenInclude(ci => ci.Product)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.ProductOrders)
                        .ThenInclude(po => po.Product)
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        public async Task<Customer?> GetCustomerAsync(string email)
            => await context.Customers
                .Include(c => c.User)
                .Include(c => c.Wallet)
                .FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);

        public async Task<bool> IsExistAsync(string email)
            => await context.Customers
                .AnyAsync(c => c.Email == email && !c.IsDeleted);

        public async Task<ICollection<Customer>> GetCustomersAsync()
            => await context.Customers
                .Include(c => c.User)
                .Include(c => c.Wallet)
                .Include(c => c.Orders)
                .Where(c => !c.IsDeleted)
                .ToListAsync();
    }
}