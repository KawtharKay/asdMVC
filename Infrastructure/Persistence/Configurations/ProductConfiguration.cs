using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Sku)
                .IsUnique();

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.QrCodeImagePath)
                .HasMaxLength(500);

            builder.Property(p => p.QrCodeBase64)
                .HasColumnType("LONGTEXT");

            builder.Property(p => p.StockQuantity)
                .HasDefaultValue(0);

            builder.Property(p => p.LowStockThreshold)
                .HasDefaultValue(5);

            // Computed properties — not stored in DB
            builder.Ignore(p => p.IsInStock);
            builder.Ignore(p => p.IsLowStock);

            // One Product → One Category (dependent side)
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
