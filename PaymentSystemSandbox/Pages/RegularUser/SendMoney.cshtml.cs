#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using System.Security.Claims;

namespace PaymentSystemSandbox.Pages.RegularUser
{
    [Authorize(Constants.Roles.RegularUser)]
    public class SendMoneyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletService _walletService;
        private readonly ILiqPayBaseService _liqPayService;

        public SendMoneyModel(ApplicationDbContext context, IWalletService walletService, ILiqPayBaseService liqPayService)
        {
            _context = context;
            _walletService = walletService;
            _liqPayService = liqPayService;
        }

        public IActionResult OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            var wallet = _context.Wallets.FirstOrDefault(it => it.UserId == userId);
            if (wallet == null)
            {
                return NotFound();
            }
            ViewData["WalletId"] = wallet.Id;
            ViewData["TaxInPercent"] = _walletService.CurrentTaxInPercent;
            PaymentTransaction = new Payment()
            {
                FromWalletId = wallet.Id,
                Price = 12M
            };
            ViewData["MaxPrice"] = Math.Max(0, wallet.Balance - _walletService.PaymentTax(wallet.Balance));
            ViewData["ToWalletId"] = new SelectList(_context.Wallets
                .Include(it => it.User)
                .Where(it => it.UserId != userId && it.User.Email != null), "Id", "User.Email");

            return Page();
        }

        [BindProperty]
        public Payment PaymentTransaction { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (
                ModelState["PaymentTransaction.FromWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.ToWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.Price"].ValidationState == ModelValidationState.Invalid)
            {
                return Page();
            }

            await _walletService.SavePendingTransactionAsync(PaymentTransaction);

            return RedirectToPage("/Index");
        }

        public async Task<PartialViewResult> OnGetPayButtonPartial(decimal amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (amount == 0 || !await _walletService.CanPaySumAsync(amount, userId))
            {
                return null;
            }
            var total = amount + _walletService.PaymentTax(amount);
            var command = new PayCommandBuilder()
                .WithAmount(total)
                .WithResultUrl("https://localhost:7151/")
                .Build();
            _liqPayService.FillWithConfiguredValues(command);

            var request = _liqPayService.EncryptApiPayload(command);
            var model = new PayButtonPartialModel()
            {
                LiqPay = request,
                Tax = _walletService.CurrentTaxInPercent,
                Total = total,
                OrderId = command.OrderId
            };
            return Partial("./_PayButtonPartial", model);
        }
    }
}
