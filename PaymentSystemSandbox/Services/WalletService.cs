using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Data.Enums;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;
using System.Transactions;

namespace PaymentSystemSandbox.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;
        
        private readonly WalletSettings _walletSettings;

        public WalletService(ApplicationDbContext context, IOptions<WalletSettings> walletSettings)
        {
            _context = context;
            _walletSettings = walletSettings.Value;
        }

        public Wallet InitiateWalletForUser(string userId)
        {
            var wallet = new Wallet()
            {
                UserId = userId,
                Balance = _walletSettings.InitialBalance
            };
            _context.Add(wallet);
            _context.SaveChanges();
            return wallet;
        }

        public async Task<Wallet> InitiateWalletForUserAsync(string userId)
        {
            var wallet = new Wallet()
            {
                UserId = userId,
                Balance = _walletSettings.InitialBalance
            };
            _context.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public decimal PaymentWithTax(decimal amount)
        {
            var result = amount * (1 + _walletSettings.CommissionInPercent / 100m);
            return result;
        }

        public void SendTransaction(PaymentTransaction paymentTransaction)
        {
            _context.Database.BeginTransaction();
            _context.Entry(paymentTransaction).Reference(it => it.FromWallet).Load();
            paymentTransaction.Status = PaymentTransactionStatus.Pending;
            paymentTransaction.IssuatedAt = DateTimeOffset.Now;
            if (paymentTransaction.FromWallet.Balance < paymentTransaction.Price)
            {

            }
            
        }

        public Task SendTransactionAsync(PaymentTransaction paymentTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
