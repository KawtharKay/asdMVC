using Application.Constants;
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
                    Id = Guid.Parse("427ACFF7-41DA-42D9-BCFA-E539BAF2E53E"),
                    Name = AppRoles.Admin,
                    CreatedBy = "asdmvc@yopmail.com",
                    DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new Role
                {
                    Id = Guid.Parse("F924A79D-D4A3-4B76-AC7B-54079C6DB4B7"),
                    Name = AppRoles.Customer,
                    CreatedBy = "asdmvc@yopmail.com",
                    DateCreated = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );
        }
    }
}
