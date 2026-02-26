namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
