using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Email)
                .IsRequired().HasMaxLength(50);
            builder.Property(x => x.Name)
                .IsRequired();
            builder.Property(x => x.Address)
                .IsRequired();
            builder.Property(x => x.PhoneNo)
                .IsRequired();
            builder.HasOne(x => x.Orders)
                .WithMany()
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
