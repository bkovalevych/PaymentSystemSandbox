using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.Models
{
    public class LiqPayResponse
    {
        public string Action { get; set; }

        public decimal AgentCommission { get; set; }

        public decimal Amount { get; set; }
        
        public string CardToken { get; set; }

        public string CompletionDate { get; set; }

        public string CreateDate { get; set; }

        public string Currency { get; set; }

        public string OrderId { get; set; }

        public string Status { get; set; }
    }
}
