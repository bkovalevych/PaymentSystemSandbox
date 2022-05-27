
using PaymentSystemSandbox.Data.Enums;

namespace PaymentSystemSandbox.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public DateTimeOffset IssuatedAt { get; set; }

        public DateTimeOffset ProcessedAt { get; set; }
        
        public PaymentTransactionStatus Status { get; set; } 

        public int FromWalletId { get; set; }

        public Wallet FromWallet { get; set; }

        public int ToWalletId { get; set; }

        public Wallet ToWallet { get; set; }

        public decimal Price { get; set; }

        public decimal TaxInPercent { get; set; }

        public decimal PriceWithTax { get; set; }
        
        public Guid OrderId { get; set; }

        public List<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
