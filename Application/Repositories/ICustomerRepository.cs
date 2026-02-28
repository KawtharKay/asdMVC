using Domain.Entities;

namespace Application.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> IsExistAsync(string email);
        Task AddAsync(Customer customer);
        Task<Customer?> GetCustomerAsync(Guid id);
        Task<ICollection<Customer>> GetCustomersAsync();
        Task<Customer?> GetCustomerAsync(string email);
    }
}
