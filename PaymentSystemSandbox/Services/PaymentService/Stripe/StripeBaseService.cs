using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Services.PaymentService.Stripe.ConfigurationModels;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystemSandbox.Services.PaymentService.Stripe
{
    public class StripeBaseService
    {
        private readonly string _host;
        private readonly string _priceId;
        public StripeBaseService(IOptions<StripeSettings> stripeSettings, IWebHostEnvironment webHost)
        {
            StripeConfiguration.ApiKey = stripeSettings.Value.PrivateKey;
            _host = webHost.WebRootPath;
            _priceId = InitDefaultPrice();
        }

        private string InitDefaultPrice()
        {
            var defaultProduct = new PriceCreateOptions()
            {
                
                Active = true,
                BillingScheme = "unit_amount_decimal",
                Currency = "uah",
                ProductData = new PriceProductDataOptions()
                {
                    Active = true,
                    Name = "Charge card"
                },
                UnitAmountDecimal = 100,
                Nickname = "default price"
            };
            
            var price = new PriceService()
                .Create(defaultProduct);
            return price.Id;
        }

        public string GetPaymentUrl(string orderId, decimal total)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                      Currency = "UAH",
                      Price = _priceId,
                      Quantity = (long)(total * 100m)
                  },
                },
                Mode = "payment",
                SuccessUrl = _host,
                CancelUrl = _host + "/cancel.html",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return session.Url;
        }
    }
}
