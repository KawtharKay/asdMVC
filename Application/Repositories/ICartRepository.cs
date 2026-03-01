using Domain.Entities;

namespace Application.Repositories
{
    public interface ICartRepository
    {
        Task AddCartAsync(Cart cart);
        Task<Cart?> GetByCustomerIdAsync(Guid customerId);
    }
}