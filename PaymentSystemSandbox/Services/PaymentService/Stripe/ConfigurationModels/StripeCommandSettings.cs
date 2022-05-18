namespace PaymentSystemSandbox.Services.PaymentService.Stripe.ConfigurationModels
{
    public class StripeCommandSettings
    {
        public string Version { get; set; }

        public string Currency { get; set; }

        public string ServerUrl { get; set; }

        public string ApiUrl { get; set; }
    }
}
