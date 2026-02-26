using Domain.Entities;

namespace Application.Repositories
{
    public interface ICartRepository
    {
        Task AddCartAsync(Cart cart);
        Task<Cart?> GetCartByIdAsync(Guid id);
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        void Update(Cart cart);

    }
}
