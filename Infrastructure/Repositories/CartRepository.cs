using Application.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        public Task AddCartAsync(Cart cart)
        {

        }

        public Task<Cart?> GetCartByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Cart cart)
        {
            throw new NotImplementedException();
        }
    }
}
