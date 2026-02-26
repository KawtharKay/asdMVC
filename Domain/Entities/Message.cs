namespace Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = default!;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
