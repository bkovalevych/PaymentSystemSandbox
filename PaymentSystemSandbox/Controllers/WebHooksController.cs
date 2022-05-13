using Microsoft.AspNetCore.Mvc;
using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using PaymentSystemSandbox.Data.Enums;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;

namespace PaymentSystemSandbox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebHooksController : ControllerBase
    {
        private readonly ILiqPayBaseService _liqPayService;
        private readonly IWalletService _walletService;
        private readonly ILogger<WebHooksController> _logger;
        public WebHooksController(ILiqPayBaseService liqPayBaseService, IWalletService walletService, 
            ILogger<WebHooksController> logger)
        {
            _liqPayService = liqPayBaseService;
            _walletService = walletService;
            _logger = logger;
        }

        [HttpPost("pending")]
        public async Task<IActionResult> Post([FromBody] PaymentTransactionDto paymentTransactionDto)
        {
            var transaction = new PaymentTransaction()
            {
                FromWalletId = paymentTransactionDto.FromWalletId,
                ToWalletId = paymentTransactionDto.ToWalletId,
                Price = paymentTransactionDto.Price,
                OrderId = paymentTransactionDto.OrderId
            };
            await _walletService.SavePendingTransactionAsync(transaction);
            return Ok("ok");
        }

        [HttpPost("liqpay")]
        public async Task<IActionResult> PostFromLiqPay([FromForm] LiqPayRequest rawLiqPay)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid payload");
                return Ok();
            }

            if (!_liqPayService.IsValid(rawLiqPay))
            {
                _logger.LogWarning("Invalid payload");
                return BadRequest();
            }
            LiqPayResponse response;
            try
            {
                response = _liqPayService.DecryptApiPayload<LiqPayResponse>(rawLiqPay);
                await ProcessLiqPay(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error on processing. Message: {0}", ex.Message);
                return BadRequest();
            }

            return Ok();
        }

        private async Task ProcessLiqPay(LiqPayResponse response)
        {
            var id = new Guid(response.OrderId);
            
            var status = response.Status switch
            {
                "success" => PaymentTransactionStatus.Confirmed,
                "sandbox" => PaymentTransactionStatus.Confirmed,
                _ => PaymentTransactionStatus.Failed
            };
            _logger.LogDebug("Start processing transaction. Status {0}. Id {1}", status, id);
            await _walletService.ProcessTransactionAsync(id, status);
        }

    }
}
