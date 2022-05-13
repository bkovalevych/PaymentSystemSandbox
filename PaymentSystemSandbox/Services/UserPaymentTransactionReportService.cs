using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;
using System.Linq.Expressions;

namespace PaymentSystemSandbox.Services
{
    public class UserPaymentTransactionReportService : IUserPaymentTransactionReportService
    {
        private readonly ApplicationDbContext _context;

        public UserPaymentTransactionReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagingList<PaymentTransaction>> GetTransactionsByUserAsync(string userId, int? top = 0, int? offset = 20, Expression<Func<PaymentTransaction, bool>> filter = null)
        {
            var result = new PagingList<PaymentTransaction>()
            {
                Top = top ?? 0,
                Offset = offset ?? 20,
            };
            var userPayments = _context.PaymentTransactions
                .Where(it => it.FromWallet.UserId == userId || it.ToWallet.UserId == userId);
            result.TotalCount = await userPayments.CountAsync();
            result.AddRange(await userPayments
                .Include(it => it.FromWallet)
                    .ThenInclude(it => it.User)
                .Include(it => it.ToWallet)
                    .ThenInclude(it => it.User)
                .Where(filter ?? (_ => true))
                .OrderByDescending( it => it.IssuatedAt)
                .Skip(result.Top)
                .Take(result.Offset).ToListAsync());

            return result;
        }

        public PagingList<PaymentTransaction> GetTransactionsByUser(string userId, int? top, int? offset, Expression<Func<PaymentTransaction, bool>> filter)
        {
            var result = new PagingList<PaymentTransaction>()
            {
                Top = top ?? 0,
                Offset = offset ?? 20,
            };
            var userPayments = _context.PaymentTransactions
                .Where(it => it.FromWallet.UserId == userId || it.ToWallet.UserId == userId);
            result.TotalCount = userPayments.Count();
            result.AddRange(userPayments
                .Include(it => it.FromWallet)
                    .ThenInclude(it => it.User)
                .Include(it => it.ToWallet)
                    .ThenInclude(it => it.User)
                .Where(filter ?? (_ => true))
                .Skip(result.Top)
                .Take(result.Offset));

            return result;
        }
    }
}
