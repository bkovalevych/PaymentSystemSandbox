using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Data.Enums;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IWalletService
    {
        decimal CurrentTaxInPercent { get; }

        Task<bool> CanPaySumAsync(decimal amount, string userId);

        Task<Wallet> InitiateWalletForUserAsync(string userId);

        decimal PaymentTax(decimal amount);


        Task SavePendingTransactionAsync(Payment paymentTransaction);

        Task ProcessTransactionAsync(Guid orderId, PaymentTransactionStatus status);
    }
}
