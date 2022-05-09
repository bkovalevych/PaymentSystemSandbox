using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IUserPaymentTransactionService
    {
        List<PaymentTransaction> GetTransactionsByUser(string userId);

        Task<List<PaymentTransaction>> GetTransactionsByUserAsync(string userId);
    }
}
