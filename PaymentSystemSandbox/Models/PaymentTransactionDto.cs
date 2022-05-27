using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystemSandbox.Models
{
    public class PaymentTransactionDto
    {
        public int FromWalletId { get; set; }

        public int ToWalletId { get; set; }

        public decimal Price { get; set; }

        public Guid OrderId { get; set; }
    }
}
