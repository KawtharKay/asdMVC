using Domain.Entities;

namespace Application.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<ICollection<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<ICollection<Order>> GetAllAsync();
    }
}