using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Configurations
{
    public class UserRoleTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.User)
                .WithMany(a => a.UserRoles)
                .HasForeignKey(a => a.UserId);

            builder.HasOne(a => a.Role)
                .WithMany(a => a.UserRoles)
                .HasForeignKey(a => a.RoleId);
        }
    }
}
