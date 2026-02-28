namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string? Fullname { get; set; }
        public string Email { get; set; } = default!;
        public string HashPassword { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public ICollection<UserConversation> UserConversations { get; set; } = new HashSet<UserConversation>();
    }
}
