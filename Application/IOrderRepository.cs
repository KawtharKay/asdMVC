using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface IOrderRepository
    {
        Task AddToDbAsync(Order order);
        Task<Order>? GetOrderAsync(Guid id);
        Task<ICollection<Order>> GetOrdersAsync();
    }
}
