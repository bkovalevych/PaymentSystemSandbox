using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;

namespace PaymentSystemSandbox.Services
{
    public class PaymentReportsService : IPaymentReportsService
    {
        private readonly ApplicationDbContext _context;

        public PaymentReportsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public TransactionReport GetReport(int? top = 0, int? offset = 20)
        {
            var result = new TransactionReport()
            {
                Offset = top ?? 0,
                Fetch = offset ?? 20,
            };
            var payments = _context.Payments.AsEnumerable();
            result.PaymentTransactions = payments
                .OrderByDescending(it => it.IssuatedAt)
                .Skip(top ?? 0)
                .Take(offset ?? 20)
                .ToList();
            result.TotalCount = payments.Count();
            result.TotalAmount = payments.Sum(it => it.PriceWithTax);
            result.TotalProfit = result.TotalAmount - payments.Sum(it => it.Price);

            return result;
        }

        public async Task<TransactionReport> GetReportAsync(int? top = 0, int? offset = 20)
        {

            var result = new TransactionReport()
            {
                Offset = top ?? 0,
                Fetch = offset ?? 20,
            };
            var payments = _context.Payments
                .Include(it => it.FromWallet)
                    .ThenInclude(it => it.User)
                .Include(it => it.ToWallet)
                    .ThenInclude(it => it.User)
                .AsEnumerable();

            result.PaymentTransactions = payments
                .OrderByDescending(it => it.IssuatedAt)
                .Skip(top ?? 0)
                .Take(offset ?? 20)
                .ToList();
            result.TotalCount = payments.Count();
            result.TotalAmount = payments.Sum(it => it.PriceWithTax);
            result.TotalProfit = result.TotalAmount - payments.Sum(it => it.Price);

            return result;
        }
    }
}
