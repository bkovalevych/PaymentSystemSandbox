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
using PaymentSystemSandbox.Services.PaymentService.Abstractions;
using PaymentSystemSandbox.Services.PaymentService.LiqPay.Models;
using PaymentSystemSandbox.Services.PaymentService.Stripe;
using System.Security.Claims;

namespace PaymentSystemSandbox.Pages.RegularUser
{
    [Authorize(Constants.Roles.RegularUser)]
    public class SendMoneyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWalletService _walletService;
        private readonly PaymentCheckoutFactory _checkoutFactory;
        private string _selectedPayment;
        public SendMoneyModel(ApplicationDbContext context, IWalletService walletService,
            PaymentCheckoutFactory checkoutFactory)
        {
            _context = context;
            _walletService = walletService;
            _checkoutFactory = checkoutFactory;
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
                Price = 30M
            };
            ViewData["MaxPrice"] = Math.Max(0, wallet.Balance - _walletService.PaymentTax(wallet.Balance));
            ViewData["ToWalletId"] = new SelectList(_context.Wallets
                .Include(it => it.User)
                .Where(it => it.UserId != userId && it.User.Email != null), "Id", "User.Email");
            
            ViewData["Payments"] = new SelectList(_checkoutFactory.AvailablePayments);
            SelectedPayment = _checkoutFactory.AvailablePayments[0];
            
            return Page();
        }

        [BindProperty]
        public Payment PaymentTransaction { get; set; }

        [BindProperty]
        public string SelectedPayment 
        { 
            get => _selectedPayment; 
            set
            {
                _selectedPayment = value;
                ViewData["MinPrice"] = _checkoutFactory.GetServiceByName(_selectedPayment).MinPrice;
            }
        }

        public IActionResult OnGetPaymentChange(string payment)
        {
            SelectedPayment = payment;
            return new JsonResult(_checkoutFactory.GetServiceByName(_selectedPayment).MinPrice);
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var total = await GetTotal(PaymentTransaction.Price);
            if (
                ModelState["PaymentTransaction.FromWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.ToWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.Price"].ValidationState == ModelValidationState.Invalid ||
                total == 0m)
            {
                return Page();
            }
            var orderId = await _walletService.SavePendingTransactionAsync(PaymentTransaction);
            var checkout = _checkoutFactory.GetServiceByName(SelectedPayment);
            return await checkout.Checkout(ViewData, total, orderId.ToString());
        }

        public async Task<PartialViewResult> OnGetPayButtonPartial(decimal amount)
        {
            var total = await GetTotal(amount);
            if (total == 0m)
            {
                return null;
            }
            var model = new PayButtonPartialModel()
            {
                Tax = _walletService.CurrentTaxInPercent,
                Total = total
            };
            return Partial("./_PayButtonPartial", model);
        }

        private async Task<decimal> GetTotal(decimal amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (amount == 0 || !await _walletService.CanPaySumAsync(amount, userId))
            {
                return 0m;
            }
            var total = amount + _walletService.PaymentTax(amount);
            return total;
        }
    }
}
