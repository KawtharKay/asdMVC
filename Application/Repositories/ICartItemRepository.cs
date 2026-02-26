using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface ICartItemRepository
    {
        Task<bool> IsExistAsync(Guid cartId, Guid productId);
        Task AddtoDbAsync(CartItem cartItem);
    }
}
