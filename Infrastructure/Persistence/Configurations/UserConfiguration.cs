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

            builder.Property(u => u.Salt)
                .IsRequired();

            builder.Property(u => u.ImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.VerificationToken)
                .HasMaxLength(4);

            builder.Property(u => u.VerificationTokenExpiry)
                .IsRequired(false);

            builder.Property(u => u.IsVerified)
                .HasDefaultValue(false);
        }
    }
}
