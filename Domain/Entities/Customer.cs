namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string PhoneNo { get; set; } = default!;
        public Cart Cart { get; set; } = default!;
        public Wallet? Wallet { get; set; }
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
    }
}
