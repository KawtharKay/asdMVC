using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IWalletRepository
    {
        Task AddWalletAsync(Wallet wallet);
        Task<Wallet?> GetByCustomerIdAsync(Guid customerId);
        void UpdateWallet(Wallet wallet);
    }
}
