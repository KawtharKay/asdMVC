namespace Domain.Entities
{
    public class Conversation : BaseEntity
    {
        public string Title { get; set; } = default!;
        public bool IsHandedOffToHuman { get; set; }
        public DateTime LastMessageAt { get; set; }
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
        public ICollection<UserConversation> UserConversations { get; set; } = new HashSet<UserConversation>();
    }
}
