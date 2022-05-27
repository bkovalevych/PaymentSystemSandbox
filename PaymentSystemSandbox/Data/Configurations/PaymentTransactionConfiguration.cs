using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Data.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasOne(it => it.Payment)
                .WithMany(it => it.PaymentTransactions)
                .HasForeignKey(it => it.PaymentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(it => it.Status)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
