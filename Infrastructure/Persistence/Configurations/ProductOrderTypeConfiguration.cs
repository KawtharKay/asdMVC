using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductOrderTypeConfiguration : IEntityTypeConfiguration<ProductOrder>
    {
        public void Configure(EntityTypeBuilder<ProductOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(a => a.Quantity)
                .IsRequired();
            builder.Property(a => a.UnitPrice)
                .IsRequired();

            builder.HasOne(a => a.Order)
                .WithMany(a => a.ProductOrders)
                .HasForeignKey(a => a.OrderId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Product)
                .WithMany(a => a.ProductOrders)
                .HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
