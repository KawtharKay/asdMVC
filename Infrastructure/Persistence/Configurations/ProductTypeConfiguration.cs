using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    internal class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x=>x.Name)
                .IsRequired().HasMaxLength(50);
            builder.Property(x => x.Price)
                .IsRequired();
            builder.Property(x=> x.Sku)
                .IsRequired();

            builder.HasMany(x=>x.ProductOrders)
                .WithOne(x=>x.Product).HasForeignKey(x=>x.Id);
            builder.HasMany(x=>x.CartItems)
                .WithOne(x=>x.Product).HasForeignKey(x=> x.Id);
        }
    }
