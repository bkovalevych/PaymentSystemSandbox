using PaymentSystemSandbox.Data.Entities;

namespace PaymentSystemSandbox.Services.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> InitiateWalletForUserAsync(string userId);
        Wallet InitiateWalletForUser(string userId);

        Task SendTransactionAsync(PaymentTransaction paymentTransaction);
        void SendTransaction(PaymentTransaction paymentTransaction);
        
        decimal PaymentTax(decimal amount);

        decimal CurrentTaxInPercent { get; }
    }
}
