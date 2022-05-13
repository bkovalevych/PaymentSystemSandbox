using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using System.Linq.Expressions;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IUserPaymentTransactionReportService
    {
        PagingList<PaymentTransaction> GetTransactionsByUser(string userId, int? top = 0, int? offset = 20, Expression<Func<PaymentTransaction, bool>> filter = null);

        Task<PagingList<PaymentTransaction>> GetTransactionsByUserAsync(string userId, int? top = 0, int? offset = 20, Expression<Func<PaymentTransaction, bool>> filter = default);
    }
}
