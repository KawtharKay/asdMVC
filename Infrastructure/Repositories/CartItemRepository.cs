using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;
        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddtoDbAsync(CartItem cartItem)
        {
            await _context.Set<CartItem>().AddAsync(cartItem);
        }

        public async Task<bool> IsExistAsync(Guid cartId, Guid productId)
        {
            return await _context.Set<CartItem>().AnyAsync(x => x.CartId == cartId && x.ProductId == productId);
        }
    }
}
