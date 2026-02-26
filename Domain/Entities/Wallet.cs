namespace Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public decimal Balance { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
