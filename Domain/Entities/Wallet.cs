namespace Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public decimal Balance { get; set; }
        public ICollection<Tranzaction> Transactions { get; set; } = new HashSet<Tranzaction>();
    }
}
