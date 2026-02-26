using Domain.Enums;

namespace Domain.Entities
{
    public class Tranzaction : BaseEntity
    {
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string? PaystackReference { get; set; }
        public string Description { get; set; } = default!;
    }

}
