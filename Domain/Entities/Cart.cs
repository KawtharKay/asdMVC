namespace Domain.Entities
{
    public class Cart
    {
        public Guid Id {  get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public decimal TotalPrice => CartItems.Sum(a => a.Quantity * a.Product.Price);
    }
}
