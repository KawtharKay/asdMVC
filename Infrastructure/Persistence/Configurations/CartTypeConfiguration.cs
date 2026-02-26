using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CartTypeConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Customer)
                .WithOne(z => z.Cart).HasForeignKey<Cart>(x => x.CustomerId);

            builder.HasMany(x => x.CartItems)
                .WithOne(z => z.Cart).HasForeignKey(x => x.CartId);

                builder.Property(x => x.TotalPrice).IsRequired();
        }
    }
}
