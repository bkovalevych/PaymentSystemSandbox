using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PaymentSystemSandbox.Data.Enums;
using PaymentSystemSandbox.Services.PaymentService.Abstractions;
using Stripe;
using Stripe.Checkout;

namespace PaymentSystemSandbox.Services.PaymentService.Stripe
{
    public class StripePaymentCheckoutAdapter : IAbstractPaymentCheckoutAdapter
    {
        private readonly StripeBaseService _stripe;
        private readonly IHttpContextAccessor _accessor;

        public StripePaymentCheckoutAdapter(StripeBaseService stripe, IHttpContextAccessor accessor)
        {
            _stripe = stripe;
            _accessor = accessor;
        }

        public string PaymentName => "Stripe";

        public decimal MinPrice => _stripe.CommandSettings.MinPrice;

        public async Task<IActionResult> Checkout(ViewDataDictionary viewData, decimal amount, string orderId)
        {
            var url = await _stripe.GetPaymentUrl(orderId, amount);
            return new RedirectResult(url);
        }

        public async Task<(Guid, PaymentTransactionStatus)> FulfillPayment(string rawRequest, CancellationToken cancellationToken)
        {
            var eventInstance = ValidatePayload(rawRequest);
            var session = eventInstance.Data.Object as Session;
           
            var status = eventInstance.Type switch
            {
                Events.CheckoutSessionAsyncPaymentFailed => PaymentTransactionStatus.Failed,
                Events.CheckoutSessionAsyncPaymentSucceeded => PaymentTransactionStatus.Confirmed,
                Events.CheckoutSessionCompleted => 
                session?.PaymentStatus == "paid"
                ? PaymentTransactionStatus.Confirmed
                : PaymentTransactionStatus.WaitingPayment,
                _ => throw new ArgumentException("event type was not defined")
            };
            var orderId = new Guid(session.Metadata["orderId"]);
           
            return (orderId, status);
        }



        private Event ValidatePayload(string rawRequest)
        {
            var eventInstance = _stripe.ValidatePayload(
                rawRequest,
                _accessor.HttpContext.Request.Headers["Stripe-Signature"]);
            if (eventInstance == null)
            {
                throw new InvalidOperationException("event is null");
            }
            return eventInstance;   
        }
    }
}
