using Domain.Enums;

namespace Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Ref { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsRead { get; set; } 
        public DateTime? ReadAt { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
