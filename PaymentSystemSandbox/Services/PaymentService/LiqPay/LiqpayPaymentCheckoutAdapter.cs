using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PaymentSystemSandbox.Data.Enums;
using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.Abstractions;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using System.Text.Json;

namespace PaymentSystemSandbox.Services.PaymentService.LiqPay
{
    public class LiqpayPaymentCheckoutAdapter : IAbstractPaymentCheckoutAdapter
    {
        private readonly ILiqPayBaseService _liqpayBaseService;

        public LiqpayPaymentCheckoutAdapter(ILiqPayBaseService liqPayBaseService)
        {
            _liqpayBaseService = liqPayBaseService;
        }

        public string PaymentName => "LiqPay";

        public decimal MinPrice => _liqpayBaseService.CommandSettings.MinPrice;

        public async Task<IActionResult> Checkout(ViewDataDictionary viewData, decimal amount, string orderId)
        {
            var command = new PayCommandBuilder()
                .WithAmount(amount)
                .WithResultUrl("https://localhost:7151/")
                .Build();
            command.OrderId = orderId;
            _liqpayBaseService.FillWithConfiguredValues(command);

            var request = _liqpayBaseService.EncryptApiPayload(command);
            return new PartialViewResult()
            {                
                ViewName = "Shared/Payments/Liqpay/_CheckoutFormPartial",
                ViewData = new ViewDataDictionary<LiqPayRequest>(viewData, request)
            };
            
        }

        public async Task<(Guid, PaymentTransactionStatus)> FulfillPayment(string rawRequest, CancellationToken cancellationToken)
        {
            var payload = ValidatePayload(rawRequest);
            var id = new Guid(payload.OrderId);

            var status = payload.Status switch
            {
                "success" => PaymentTransactionStatus.Confirmed,
                "sandbox" => PaymentTransactionStatus.Confirmed,
                _ => PaymentTransactionStatus.Failed
            };

            return (id, status);
        }

        private LiqPayResponse ValidatePayload(string rawRequest)
        {
            var request = JsonSerializer.Deserialize<LiqPayRequest>(rawRequest);
            if (request == null)
            {
                throw new InvalidOperationException("json error deserialization");
            }
            if (!_liqpayBaseService.IsValid(request))
            {
                throw new InvalidOperationException("invalid signature");
            }
            var payload = _liqpayBaseService.DecryptApiPayload<LiqPayResponse>(request);
            return payload;
        }
    }
}
