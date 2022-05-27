using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Pages.Admin
{
    public class ReportPaymentDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public ReportPaymentDetailsModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Payment Payment { get; set; }
        
        public async Task OnGetAsync(int paymentId)
        {
            Payment = await _ctx.Payments
                .Include(it => it.ToWallet)
                    .ThenInclude(it => it.User)
                .Include(it => it.FromWallet)
                    .ThenInclude(it => it.User)
                .Include(it => it.PaymentTransactions)
                .SingleAsync(it => it.Id == paymentId);
        }
    }
}
