using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.HasIndex(x => x.Email).IsUnique();


            builder.Property(x => x.Email)
                .IsRequired().HasMaxLength(60);

            builder.HasOne(a => a.Customer)
                .WithOne(a => a.User)
                .HasForeignKey<Customer>(a => a.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.UserRoles)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
