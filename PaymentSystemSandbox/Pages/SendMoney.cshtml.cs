#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Pages
{
    public class SendMoneyModel : PageModel
    {
        private readonly PaymentSystemSandbox.Data.ApplicationDbContext _context;
        public Wallet Wallet { get; set; }
        public SendMoneyModel(PaymentSystemSandbox.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }
            
            Wallet = _context.Wallets.FirstOrDefault(it => it.UserId == userId);
            ViewData["WalletId"] = Wallet.Id;
            PaymentTransaction = new PaymentTransaction()
            {
                FromWalletId = Wallet.Id,
                Price = 12M
            };
            if (Wallet == null)
            {
                Wallet = new Wallet()
                {
                    Balance = 1000,
                    UserId = userId
                };
                _context.Add(Wallet);
                _context.SaveChanges();
            }
            ViewData["MaxPrice"] = Wallet.Balance;
            ViewData["ToWalletId"] = new SelectList(_context.Wallets
                .Include(it => it.User)
                .Where(it => it.UserId != userId), "Id", "User.Email");
            return Page();
        }

        [BindProperty]
        public PaymentTransaction PaymentTransaction { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (
                ModelState["PaymentTransaction.FromWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.ToWalletId"].ValidationState == ModelValidationState.Invalid ||
                ModelState["PaymentTransaction.Price"].ValidationState == ModelValidationState.Invalid)
            {
                return Page();
            }
            _context.PaymentTransactions.Add(PaymentTransaction);
            
            await _context.SaveChangesAsync();
            
            

            return RedirectToPage("./Index");
        }
    }
}
