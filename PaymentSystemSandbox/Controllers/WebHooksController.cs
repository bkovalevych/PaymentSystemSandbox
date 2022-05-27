using Microsoft.AspNetCore.Mvc;
using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.Abstractions;

namespace PaymentSystemSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebHooksController : ControllerBase
    {
        private readonly PaymentCheckoutFactory _factory;
        private readonly IWalletService _walletService;
        private readonly ILogger<WebHooksController> _logger;

        public WebHooksController(PaymentCheckoutFactory factory, IWalletService walletService,
            ILogger<WebHooksController> logger)
        {
            _factory = factory;
            _walletService = walletService;
            _logger = logger;
        }

        [HttpPost("liqpay")]
        public async Task<IActionResult> PostFromLiqPay(CancellationToken cancellationToken)
        {
            return await FulfillOrderByPaymentName("LiqPay", cancellationToken);
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> PostFromStripe(CancellationToken cancellationToken)
        {
            return await FulfillOrderByPaymentName("Stripe", cancellationToken);
        }

        private async Task<IActionResult> FulfillOrderByPaymentName(string paymentName, CancellationToken cancellationToken)
        {
            string rawLiqPay = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var service = _factory.GetServiceByName(paymentName);
                var (orderId, status) = await service.FulfillPayment(rawLiqPay, cancellationToken);
                await _walletService.ProcessTransactionAsync(orderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error on processing. Payment: {0}. Message: {1}", paymentName, ex.Message);
                return BadRequest();
            }

            return Ok();
        }
    }
}
