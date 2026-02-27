using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Name)
                .IsRequired().HasMaxLength(50);
            builder.Property(x => x.Price)
                .IsRequired();
            builder.Property(x => x.Sku)
                .IsRequired();

            builder.Property(p => p.QrCodeImagePath)
                .HasMaxLength(500);

            builder.HasMany(x => x.ProductOrders)
                .WithOne(x => x.Product).HasForeignKey(x => x.Id);
            builder.HasMany(x => x.CartItems)
                .WithOne(x => x.Product).HasForeignKey(x => x.Id);
        }
    }
}
