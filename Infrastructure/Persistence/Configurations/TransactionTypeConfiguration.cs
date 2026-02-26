using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Transactions;

namespace Infrastructure.Persistence.Configurations
{
    public class TransactionTypeConfiguration : IEntityTypeConfiguration<Tranzaction>
    {
        public void Configure(EntityTypeBuilder<Tranzaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(v => v.Amount)
                .IsRequired();

            builder.Property(y => y.PaystackReference)
                .IsRequired();

            builder.Property(t => t.Type)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired();

             builder.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(d => d.Wallet)
                .WithMany(p => p.Transactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
