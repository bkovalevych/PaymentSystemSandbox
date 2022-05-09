#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Pages
{
    public class WalletDetailsModel : PageModel
    {
        private readonly PaymentSystemSandbox.Data.ApplicationDbContext _context;

        public WalletDetailsModel(PaymentSystemSandbox.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Wallet Wallet { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return NotFound();
            }

            Wallet = await _context.Wallets
                .Include(w => w.User).FirstOrDefaultAsync(m => m.UserId == userId);

            if (Wallet == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
