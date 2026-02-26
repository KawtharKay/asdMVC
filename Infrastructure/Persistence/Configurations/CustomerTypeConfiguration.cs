using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name)
                .IsRequired();
            builder.Property(x => x.Address)
                .IsRequired();
            builder.Property(x => x.PhoneNo)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(x => x.Customer).HasForeignKey<Customer>(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Cart)
                .WithOne(x => x.Customer).HasForeignKey<Cart>(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Wallet)
                .WithOne(x => x.Customer).HasForeignKey<Wallet>(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}