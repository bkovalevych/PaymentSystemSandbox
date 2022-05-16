using PaymentSystemSandbox.Data.Enums;

namespace PaymentSystemSandbox.Data.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public int PaymentId { get; set; }

        public Payment Payment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public PaymentTransactionStatus Status { get; set; }
    }
}
