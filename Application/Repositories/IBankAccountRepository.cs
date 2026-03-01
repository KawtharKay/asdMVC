using Domain.Entities;

namespace Application.Repositories
{
    public interface IBankAccountRepository
    {
        Task AddAsync(BankAccount bankAccount);
        Task<BankAccount?> GetByIdAsync(Guid id);
        Task<ICollection<BankAccount>> GetByCustomerIdAsync(Guid customerId);
    }
}