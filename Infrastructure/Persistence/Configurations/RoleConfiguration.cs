using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567891"),
                    Name = "Admin",
                    CreatedBy = "System",
                    DateCreated = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567892"),
                    Name = "Customer",
                    CreatedBy = "System",
                    DateCreated = DateTime.UtcNow
                }
            );
        }
    }
}
