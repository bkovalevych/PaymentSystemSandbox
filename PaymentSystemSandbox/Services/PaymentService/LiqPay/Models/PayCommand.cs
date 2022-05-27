namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.Models
{
    public class PayCommand : BaseLiqPayCommand
    {   
        public string Action { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string OrderId { get; set; }
        
        public string Language { get; set; }
    }
}
