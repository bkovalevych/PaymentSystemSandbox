using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Data.Entities;
using PaymentSystemSandbox.Data.Enums;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services.Interfaces;

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

        public decimal CurrentTaxInPercent => _walletSettings.CommissionInPercent;

        public async Task<bool> CanPaySumAsync(decimal amount, string userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return false;
            }
            return CanPaySum(amount, wallet.Balance);
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

        public decimal PaymentTax(decimal amount)
        {
            var result = amount * _walletSettings.CommissionInPercent / 100m;
            return result;
        }


        public async Task SavePendingTransactionAsync(PaymentTransaction paymentTransaction)
        {
            paymentTransaction.IssuatedAt = DateTimeOffset.Now;
            paymentTransaction.Status = PaymentTransactionStatus.Pending;
            paymentTransaction.TaxInPercent = _walletSettings.CommissionInPercent;
            paymentTransaction.PriceWithTax = paymentTransaction.Price + PaymentTax(paymentTransaction.Price);
            _context.PaymentTransactions.Add(paymentTransaction);
            await _context.SaveChangesAsync();
        }

        public async Task ProcessTransactionAsync(Guid orderId, PaymentTransactionStatus status)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (transaction == null)
            {
                return;
            }
            transaction.Status = status;
            if (status == PaymentTransactionStatus.Confirmed)
            {
                await ConfirmTransactionAsync(transaction);
            }
            _context.PaymentTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        private async Task ConfirmTransactionAsync(PaymentTransaction paymentTransaction)
        {
            await _context.Database.BeginTransactionAsync();
            var wallet = await _context.Wallets.FirstOrDefaultAsync(it => it.Id == paymentTransaction.FromWalletId);
            var toWallet = await _context.Wallets.FirstOrDefaultAsync(it => it.Id == paymentTransaction.ToWalletId);
            
            if (wallet.Balance < paymentTransaction.PriceWithTax)
            {
                return;
            }
            wallet.Balance -= paymentTransaction.PriceWithTax;
            toWallet.Balance += paymentTransaction.Price;
            try
            {
                _context.Wallets.Update(wallet);
                _context.Wallets.Update(toWallet);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
            }

            await _context.Database.CommitTransactionAsync();
        }

        private bool CanPaySum(decimal amount, decimal balance)
        {
            return balance >= amount + PaymentTax(amount);
        }
    }
}
