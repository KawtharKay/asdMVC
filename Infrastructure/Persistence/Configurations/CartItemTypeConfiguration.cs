

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CartItemTypeConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Cart)
                .WithMany(z => z.CartItems).HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany(z => z.CartItems).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);

             builder.Property(x => x.Quantity).IsRequired();
        }
    }
}
