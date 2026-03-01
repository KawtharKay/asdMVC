using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WalletRepository(AppDbContext context) : IWalletRepository
    {
        public async Task AddWalletAsync(Wallet wallet)
            => await context.Wallets.AddAsync(wallet);

        public async Task<Wallet?> GetByIdAsync(Guid id)
            => await context.Wallets
                .Include(w => w.Customer)
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

        public async Task<Wallet?> GetByCustomerIdAsync(Guid customerId)
            => await context.Wallets
                .Include(w => w.Customer)
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.CustomerId == customerId && !w.IsDeleted);
    }
}