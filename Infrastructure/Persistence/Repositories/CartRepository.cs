using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CartRepository(AppDbContext context) : ICartRepository
    {
        public async Task AddCartAsync(Cart cart)
            => await context.Carts.AddAsync(cart);

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId)
            => await context.Carts
                .Include(c => c.CartItems.Where(ci => !ci.IsDeleted))
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsDeleted);
    }
}