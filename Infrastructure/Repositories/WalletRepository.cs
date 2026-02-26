

using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;
        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddWalletAsync(Wallet wallet)
        {
            await _context.AddAsync(wallet);
        }

        public async Task<Wallet?> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Wallets.Include(x => x.Transactions).Include(x => x.Customer).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
        }
    }
}
