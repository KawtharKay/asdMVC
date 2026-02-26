
using Application.Common.Pagenation;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionsAsync(Tranzaction transaction)
        {

             await _context.Tranzactions.AddAsync(transaction);
        }

        public async Task<Tranzaction?> GetTransactionsAsync(Guid transactionId)
        {
            return await _context.Tranzactions.Include(t => t.Wallet)
                                              .FirstOrDefaultAsync(t => t.Id == transactionId);
        }

        

        public async Task<PagenatedList<Tranzaction>> GetTransactionsByWalletIdAsync(PageRequest request, Guid walletId)
        {
            var query = _context.Tranzactions.AsQueryable();
            var totalCount = await query.CountAsync();

            {
                var ttt = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                return new PagenatedList<Tranzaction>
                {
                    Items = await ttt.ToListAsync(),
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
        }
    }
}
