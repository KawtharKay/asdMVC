using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddToDbAsync(Customer customer)
        {
            await _context.Set<Customer>().AddAsync(customer);
        }

        public async Task<Customer?> GetCustomerAsync(Guid id)
        {
            return await _context.Set<Customer>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ICollection<Customer>> GetCustomersAsync()
        {
            return await _context.Set<Customer>().ToListAsync();
        }

        public async Task<bool> IsExistAsync(string email)
        {
            return await _context.Set<Customer>().AllAsync(x => x.Email == email);
        }
    }
}
