namespace PaymentSystemSandbox.Services.PaymentService.LiqPay.ConfigurationModels
{
    public class LiqPayCommandSettings
    {
        public string Version { get; set; }

        public string Currency { get; set; }
        
        public string ServerUrl { get; set; }

        public string ApiUrl { get; set; }

        public decimal MinPrice { get; set; }
    }
}
