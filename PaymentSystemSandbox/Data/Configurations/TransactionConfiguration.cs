using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasOne(transaction => transaction.ToWallet)
                .WithMany()
                .HasForeignKey(transaction => transaction.ToWalletId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(transaction => transaction.FromWallet)
                .WithMany()
                .HasForeignKey(transaction => transaction.FromWalletId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(transaction => transaction.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(transaction => transaction.Price)
                .IsRequired()
                .HasColumnType($"decimal(12, 2)");
        }
    }
}
