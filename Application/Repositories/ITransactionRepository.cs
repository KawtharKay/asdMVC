

using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface ITransactionRepository 
    {
        Task AddTransactionsAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetTransactionsAsync(Guid transactionId);
        Task<PagenatedList<WalletTransaction>> GetTransactionsByWalletIdAsync(PageRequest request, Guid walletId);

    }
}
