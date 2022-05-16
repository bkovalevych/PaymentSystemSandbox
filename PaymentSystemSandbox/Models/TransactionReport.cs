using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Models
{
    public class TransactionReport
    {
        public List<Payment> PaymentTransactions { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalProfit { get; set; }

        public int TotalCount { get; set; }

        public int Offset { get; set; }

        public int Top { get; set; }
    }
}
