using Domain.Entities;

namespace Application.Repositories
{
    public interface IWalletRepository
    {
        Task AddWalletAsync(Wallet wallet);
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<Wallet?> GetByCustomerIdAsync(Guid customerId);
    }
}