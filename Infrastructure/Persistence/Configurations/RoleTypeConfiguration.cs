using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class RoleTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex (x => x.Name).IsUnique();

            builder.Property(x => x.Name).IsRequired();

            //builder.HasMany(x => x.UserRoles)
            //    .WithOne(x => x.Role).HasForeignKey(x => x.Id);
        }
    }
}
