using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;

namespace PaymentSystemSandbox.Models
{
    public class PayButtonPartialModel
    {
        public LiqPayRequest LiqPay { get; set; }

        public decimal Tax { get; set; }

        public decimal Total { get; set; }

        public string OrderId { get; set; }
    }
}
