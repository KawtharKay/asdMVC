namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string HashPassword { get; set; } = default!;
        public string Salt { get; set; } = default!;
        public string Fullname { get; set; } = default!;
        public string ProfilePictureUrl { get; set; } = default!;
        public Customer? Customer { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
