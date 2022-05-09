#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Pages.RegularUser
{
    public class UsersModel : PageModel
    {
        private readonly PaymentSystemSandbox.Data.ApplicationDbContext _context;

        public UsersModel(PaymentSystemSandbox.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PaymentTransaction> PaymentTransaction { get;set; }

        public async Task OnGetAsync()
        {
            PaymentTransaction = await _context.PaymentTransactions
                .Include(p => p.FromWallet)
                .Include(p => p.ToWallet).ToListAsync();
        }
    }
}
