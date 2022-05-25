using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Services.PaymentService.Stripe.ConfigurationModels;
using Stripe;
using Stripe.Checkout;

namespace PaymentSystemSandbox.Services.PaymentService.Stripe
{
    public class StripeBaseService
    {
        private readonly string _priceId;
        private readonly string _accountId;
        private readonly StripeSettings _settings;
        public StripeBaseService(IOptions<StripeSettings> stripeSettings, 
            IOptions<StripeCommandSettings> stripeCommandSettings,
            IHttpContextAccessor contextAccessor)
        {
            StripeConfiguration.ApiKey = stripeSettings.Value.PrivateKey;
            _accountId = stripeSettings.Value.AccountId;
            _priceId = InitDefaultPrice();
            _settings = stripeSettings.Value;
            CommandSettings = stripeCommandSettings.Value;
        }

        public StripeCommandSettings CommandSettings { get; }


        private string InitDefaultPrice()
        {
            var search = new PriceSearchOptions()
            {
                Limit = 1, 
                Query = "active:'true' AND metaData['name']:'Charge card'"
            };
            var priceService = new PriceService();
            var resultSearch = priceService.Search(search);
            if (resultSearch.Data != null && resultSearch.Data.FirstOrDefault() is Price foundPrice)
            {
                return foundPrice.Id;
            }

            var defaultProduct = new PriceCreateOptions()
            {
                Active = true,
                BillingScheme = "per_unit",
                Currency = "uah",
                ProductData = new PriceProductDataOptions()
                {
                    Active = true,
                    Name = "Charge card",
                },
                UnitAmountDecimal = 1m,
                Nickname = "default price",
                Metadata = new Dictionary<string, string>()
                {
                    ["name"] = "Charge card"
                }
            };

            var price = priceService.Create(defaultProduct);
            return price.Id;
        }

        public async Task<string> GetPaymentUrl(string orderId, decimal total)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                      Price = _priceId,
                      Quantity = (long)(total * 100m)
                  },
                },
                Metadata = new Dictionary<string, string>()
                {
                    ["orderId"] = orderId
                },
                Mode = "payment",
                SuccessUrl = CommandSettings.SuccessUrl,
                CancelUrl = CommandSettings.SuccessUrl
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options, new RequestOptions() { StripeAccount = _accountId});
            return session.Url;
        }

        public Event ValidatePayload(string rawRequest, string signature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
              rawRequest,
              signature,
              _settings.WebHookCheckoutSecretKey
            );

            return stripeEvent;
        }
    }
}
