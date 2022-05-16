using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
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

            builder.Property(transaction => transaction.TaxInPercent)
                .IsRequired()
                .HasColumnType($"decimal(5, 2)");

            builder.Property(transaction => transaction.PriceWithTax)
                .IsRequired()
                .HasColumnType($"decimal(12, 2)");
        }
    }
}
