
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.HasOne(w => w.Customer)
                .WithOne(c => c.Wallet)
                .HasForeignKey<Wallet>(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
