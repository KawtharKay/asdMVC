namespace Domain.Entities
{
    public class BankAccount : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public string BankName { get; set; } = default!;
        public string BankCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string AccountName { get; set; } = default!;
        public string? RecipientCode { get; set; }
        public bool IsDefault { get; set; } 
    }
}
