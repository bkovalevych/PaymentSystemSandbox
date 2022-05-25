using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PaymentSystemSandbox.Data.Enums;

namespace PaymentSystemSandbox.Services.PaymentService.Abstractions
{
    public interface IAbstractPaymentCheckoutAdapter
    {
        string PaymentName { get; }

        decimal MinPrice { get; }

        Task<IActionResult> Checkout(ViewDataDictionary viewData, decimal amount, string orderId);

        Task<(Guid, PaymentTransactionStatus)> FulfillPayment(string rawRequest, CancellationToken cancellationToken);
    }
}
