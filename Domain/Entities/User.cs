namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Fullname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string HashPassword { get; set; } = default!;
        public string Salt { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public Customer? Customer { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public ICollection<UserConversation> ConversationParticipants { get; set; } = new HashSet<UserConversation>();
    }
}
