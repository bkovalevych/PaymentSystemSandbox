namespace PaymentSystemSandbox.Services.PaymentService.Stripe.ConfigurationModels
{
    public class StripeSettings
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
        
        public string AccountId { get; set; }

        public string WebHookCheckoutSecretKey { get; set; }
    }
}
