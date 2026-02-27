using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.BankName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.BankCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.AccountNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.AccountName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.RecipientCode)
                .HasMaxLength(100);

            builder.HasOne(b => b.Customer)
                .WithMany(c => c.BankAccounts)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
