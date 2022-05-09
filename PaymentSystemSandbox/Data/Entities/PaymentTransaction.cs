using System.Transactions;

namespace PaymentSystemSandbox.Data.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public DateTimeOffset IssuatedAt { get; set; }

        public DateTimeOffset ProcessedAt { get; set; }
        
        public TransactionStatus Status { get; set; } 

        public int FromWalletId { get; set; }

        public Wallet FromWallet { get; set; }

        public int ToWalletId { get; set; }

        public Wallet ToWallet { get; set; }

        public decimal Price { get; set; }
    }
}
