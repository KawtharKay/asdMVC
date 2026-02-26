namespace Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public  Guid CartId { get; set; }
        public Cart Cart { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
