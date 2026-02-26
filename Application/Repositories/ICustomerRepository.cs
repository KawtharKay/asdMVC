using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> IsExistAsync(string email);
        Task AddToDbAsync(Customer customer);
        Task<Customer?> GetCustomerAsync(Guid id);
        Task<ICollection<Customer>> GetCustomersAsync();
    }
}
