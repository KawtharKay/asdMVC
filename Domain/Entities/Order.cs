using Domain.Enums;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNo { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string DeliveryAddress { get; set; } = default!;
        public ICollection<ProductOrder> ProductOrders { get; set; } = new HashSet<ProductOrder>();
    }
}
