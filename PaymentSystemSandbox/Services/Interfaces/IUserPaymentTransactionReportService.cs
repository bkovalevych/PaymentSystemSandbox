using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Models;
using System.Linq.Expressions;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IUserPaymentTransactionReportService
    {
        PagingList<Payment> GetTransactionsByUser(string userId, int? top = 0, int? offset = 20, Expression<Func<Payment, bool>> filter = null);

        Task<PagingList<Payment>> GetTransactionsByUserAsync(string userId, int? top = 0, int? offset = 20, Expression<Func<Payment, bool>> filter = default);
    }
}
