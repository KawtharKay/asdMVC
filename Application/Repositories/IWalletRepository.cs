using Domain.Entities;

namespace Application.Repositories
{
    public interface IWalletRepository
    {
        Task AddWalletAsync(Wallet wallet);
        Task<Wallet?> GetByCustomerIdAsync(Guid customerId);
        void UpdateWallet(Wallet wallet);
    }
}
