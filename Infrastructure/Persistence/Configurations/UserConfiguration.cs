using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Fullname)
                .HasMaxLength(100);

            builder.Property(u => u.HashPassword)
                .IsRequired();

            builder.Property(u => u.ImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.VerificationToken)
                .HasMaxLength(4);

            builder.Property(u => u.VerificationTokenExpiry)
                .IsRequired(false);

            builder.Property(u => u.IsVerified)
                .HasDefaultValue(false);

            builder.HasData(new User
            {
                Id = Guid.Parse("E0560D58-6052-45F7-8406-08D4B835BBEF"),
                Fullname = "Super Admin",
                Email = "asdmvc@yopmail.com",
                HashPassword = "AQAAAAIAAYagAAAAEPrO+4LQhsog1kkgNeG3EBukme08zuLh0j0eH5KRcj6HFa2t23QrzOACjVeMfMkuBQ==",
                IsVerified = true,
                CreatedBy = "asdmvc@yopmail.com",
                DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
