using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WalletTransactionRepository(AppDbContext context)
        : IWalletTransactionRepository
    {
        public async Task AddAsync(WalletTransaction transaction)
            => await context.Transactions.AddAsync(transaction);

        public async Task<WalletTransaction?> GetByIdAsync(Guid id)
            => await context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        public async Task<WalletTransaction?> GetByReferenceAsync(string reference)
            => await context.Transactions
                .Include(t => t.Wallet)
                    .ThenInclude(w => w.Customer)
                .FirstOrDefaultAsync(t =>
                    t.PaystackReference == reference && !t.IsDeleted);

        public async Task<ICollection<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
            => await context.Transactions
                .Where(t => t.WalletId == walletId && !t.IsDeleted)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();

        public async Task<ICollection<WalletTransaction>> GetAllAsync()
            => await context.Transactions
                .Include(t => t.Wallet)
                    .ThenInclude(w => w.Customer)
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
    }
}