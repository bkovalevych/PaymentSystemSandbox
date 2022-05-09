using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Data.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasOne(wallet => wallet.User)
                .WithOne()
                .HasForeignKey<Wallet>(wallet => wallet.UserId)
                .IsRequired();
            builder.Property(wallet => wallet.Balance)
                .IsRequired()
                .HasColumnType($"decimal(12, 2)");
        }
    }
}
