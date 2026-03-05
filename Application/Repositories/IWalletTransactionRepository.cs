using Domain.Entities;

namespace Application.Repositories
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction transaction);
        Task<WalletTransaction?> GetByIdAsync(Guid id);
        Task<WalletTransaction?> GetByReferenceAsync(string reference);
        Task<ICollection<WalletTransaction>> GetByWalletIdAsync(Guid walletId);
        Task<ICollection<WalletTransaction>> GetAllAsync();
    }
}