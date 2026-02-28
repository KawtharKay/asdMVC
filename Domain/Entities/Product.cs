namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Sku { get; set; } = default!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public int StockQuantity { get; set; } 
        public int LowStockThreshold { get; set; }
        public bool IsInStock => StockQuantity > 0;
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
        public string? QrCodeImagePath { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public ICollection<ProductOrder> ProductOrders { get; set; } = new HashSet<ProductOrder>();
    }
}
