using Domain.Entities;

namespace Application.Repositories
{
    public interface ICartItemRepository
    {
        Task AddAsync(CartItem cartItem);
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId);
    }
}