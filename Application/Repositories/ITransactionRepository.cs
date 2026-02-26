

using Application.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface ITransactionRepository 
    {
        Task AddTransactionsAsync(Tranzaction transaction);
        Task<Tranzaction?> GetTransactionsAsync(Guid transactionId);
        Task<PagenatedList<Tranzaction>> GetTransactionsByWalletIdAsync(PageRequest request, Guid walletId);

    }
}
