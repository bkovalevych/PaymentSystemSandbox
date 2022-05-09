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
using System.Security.Claims;

namespace PaymentSystemSandbox.Pages.RegularUser
{
    [Authorize(Constants.Roles.Admin)]
    public class SendMoneyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletService _walletService;

        public SendMoneyModel(ApplicationDbContext context, IWalletService walletService)
        {
            _context = context;
            _walletService = walletService;
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
                wallet = _walletService.InitiateWalletForUser(userId);
            }
            ViewData["WalletId"] = wallet.Id;
            ViewData["TaxInPercent"] = _walletService.CurrentTaxInPercent;
            PaymentTransaction = new PaymentTransaction()
            {
                FromWalletId = wallet.Id,
                Price = 12M
            };
            ViewData["MaxPrice"] = Math.Max(0, wallet.Balance - _walletService.PaymentTax(wallet.Balance));
            ViewData["ToWalletId"] = new SelectList(_context.Wallets
                .Include(it => it.User)
                .Where(it => it.UserId != userId), "Id", "User.Email");

            return Page();
        }

        [BindProperty]
        public PaymentTransaction PaymentTransaction { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (
                ModelState["PaymentTransaction.FromWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.ToWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.Price"].ValidationState == ModelValidationState.Invalid)
            {
                return Page();
            }
            await _walletService.SendTransactionAsync(PaymentTransaction);

            return RedirectToPage("./Index");
        }
    }
}
