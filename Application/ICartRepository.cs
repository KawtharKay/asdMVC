using Domain.Entities;

namespace Application
{
    public interface ICartRepository
    {
        Task AddCartAsync(Cart cart);
        Task<Cart?> GetCartByIdAsync(Guid id);
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        void Update(Cart cart);

    }
}
