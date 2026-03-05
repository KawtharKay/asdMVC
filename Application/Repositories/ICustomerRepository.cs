using Domain.Entities;

namespace Application.Repositories
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<Customer?> GetCustomerAsync(Guid id);
        Task<Customer?> GetCustomerAsync(string email);
        Task<bool> IsExistAsync(string email);
        Task<ICollection<Customer>> GetCustomersAsync();
    }
}