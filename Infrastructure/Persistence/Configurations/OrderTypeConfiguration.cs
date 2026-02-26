using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.OrderNo);

            builder.Property(x => x.OrderNo)
                .IsRequired();
            builder.HasOne(x => x.ProductOrders)
                .WithMany()
                .HasForeignKey(x => x.CustomerId);
        }
    }
}
