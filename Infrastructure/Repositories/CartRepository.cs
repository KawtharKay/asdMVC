using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        public CartRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCartAsync(Cart cart)
        {
            await _context.Set<Cart>().AddAsync(cart);
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return await _context.Set<Cart>().ToListAsync();
        }

        public async Task<Cart?> GetCartByIdAsync(Guid id)
        {
            return await _context.Set<Cart>().FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Set<Cart>().Include(x => x.Customer).Include(x => x.CartItems).ThenInclude(x => x.Product).SingleOrDefaultAsync(x => x.CustomerId == userId);
        }

        public void Update(Cart cart)
        {
            _context.Set<Cart>().Update(cart);
        }
    }
}
