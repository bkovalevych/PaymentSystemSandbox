#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;

namespace PaymentSystemSandbox.Pages.RegularUser
{
    public class YourPaymentTransactionsModel : PageModel
    {
        private readonly PaymentSystemSandbox.Data.ApplicationDbContext _context;
        private readonly IUserPaymentTransactionService _transactionService;

        public YourPaymentTransactionsModel(ApplicationDbContext context, IUserPaymentTransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public PagingList<PaymentTransaction> PaymentTransaction { get;set; }

        public string NextDisabled { get; set; } = "";
        public int NextTopIndex { get; set; } = 0;

        public string PrevDisabled { get; set; } = "";
        public int PrevTopIndex { get; set; } = 0;
        
        [BindProperty]
        public string PaymentType { get; set; }

        public List<string> PaymentTypes => new List<string>()
        {
            "All types",
            "From me",
            "To me"
        };

        public async Task OnGetAsync(int? top, int? offset, string paymentType)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return;
            }
            var list = new SelectList(new List<string>()
            {
                "All types",
                "From me",
                "To me"
            });
            PaymentType = paymentType;
            if (list.FirstOrDefault(it => it.Value == PaymentType) is SelectListItem it)
            {
                it.Selected = true;
            };
            ViewData["PaymentTypes"] = list; 
            Expression<Func<PaymentTransaction, bool>> filter = PaymentType switch
            {
                "From me" => it => it.FromWallet.UserId == userId,
                "To me" => it => it.ToWallet.UserId == userId,
                _ => _ => true,
            };
            PaymentTransaction = await _transactionService.GetTransactionsByUserAsync(userId, top, offset, filter);
            if (PaymentTransaction.Top + PaymentTransaction.Offset > PaymentTransaction.TotalCount)
            {
                NextDisabled = "disabled";
            }
            if (PaymentTransaction.Top == 0)
            {
                PrevDisabled = "disabled";
            }

            NextTopIndex = PaymentTransaction.Top + PaymentTransaction.Offset;
            PrevTopIndex = Math.Max(0, PaymentTransaction.Top - PaymentTransaction.Offset);
        }
    }
}
