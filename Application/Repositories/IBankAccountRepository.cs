using Domain.Entities;

namespace Application.Repositories
{
    public interface IBankAccountRepository
    {
        Task AddAsync(BankAccount bankAccount);
        Task<BankAccount?> GetBankAccountAsync(Guid customerId);
        Task<IEnumerable<BankAccount>> GetAllBankAccountsAsync();
        Task<IEnumerable<BankAccount>> GetAllBankAccountsByCustomerAsync(Guid customerId);
    }
}