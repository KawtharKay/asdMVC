using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class BankAccountRepository(AppDbContext context) : IBankAccountRepository
    {
        public async Task AddAsync(BankAccount bankAccount)
            => await context.BankAccounts.AddAsync(bankAccount);

        public async Task<BankAccount?> GetByIdAsync(Guid id)
            => await context.BankAccounts
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

        public async Task<ICollection<BankAccount>> GetByCustomerIdAsync(Guid customerId)
            => await context.BankAccounts
                .Where(b => b.CustomerId == customerId && !b.IsDeleted)
                .OrderByDescending(b => b.IsDefault)
                .ToListAsync();
    }
}