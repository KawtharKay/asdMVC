namespace Domain.Entities
{
    public class ProductOrder : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
