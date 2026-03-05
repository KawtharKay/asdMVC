using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CartItemRepository(AppDbContext context) : ICartItemRepository
    {
        public async Task AddAsync(CartItem cartItem)
            => await context.CartItems.AddAsync(cartItem);

        public async Task<CartItem?> GetByIdAsync(Guid id)
            => await context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id && !ci.IsDeleted);

        public async Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId)
            => await context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartId == cartId &&
                    ci.ProductId == productId &&
                    !ci.IsDeleted);
    }
}