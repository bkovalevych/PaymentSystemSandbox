using Microsoft.EntityFrameworkCore;
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

        public decimal CurrentTaxInPercent => _walletSettings.CommissionInPercent;

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

        public decimal PaymentTax(decimal amount)
        {
            var result = amount * _walletSettings.CommissionInPercent / 100m;
            return result;
        }

        public void SendTransaction(PaymentTransaction paymentTransaction)
        {
            _context.Database.BeginTransaction();
            var wallet = _context.Wallets.FirstOrDefault(it => it.Id == paymentTransaction.FromWalletId);
            var toWallet = _context.Wallets.FirstOrDefault(it => it.Id == paymentTransaction.ToWalletId);
            paymentTransaction.Status = PaymentTransactionStatus.Confirmed;
            paymentTransaction.IssuatedAt = DateTimeOffset.Now;
            paymentTransaction.TaxInPercent = _walletSettings.CommissionInPercent;
            paymentTransaction.PriceWithTax = paymentTransaction.Price + PaymentTax(paymentTransaction.Price);

            if (wallet.Balance < paymentTransaction.PriceWithTax)
            {
                return;
            }
            wallet.Balance -= paymentTransaction.PriceWithTax;
            toWallet.Balance += paymentTransaction.Price;
            try
            {
                _context.Add(paymentTransaction);
                _context.Wallets.Update(wallet);
                _context.Wallets.Update(toWallet);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
            }

            _context.Database.CommitTransaction();
        }

        public async Task SendTransactionAsync(PaymentTransaction paymentTransaction)
        {
            await _context.Database.BeginTransactionAsync();
            var wallet = await _context.Wallets.FirstOrDefaultAsync(it => it.Id == paymentTransaction.FromWalletId);
            var toWallet = await _context.Wallets.FirstOrDefaultAsync(it => it.Id == paymentTransaction.ToWalletId);
            paymentTransaction.Status = PaymentTransactionStatus.Confirmed;
            paymentTransaction.IssuatedAt = DateTimeOffset.Now;
            paymentTransaction.TaxInPercent = _walletSettings.CommissionInPercent;
            paymentTransaction.PriceWithTax = paymentTransaction.Price + PaymentTax(paymentTransaction.Price);
            
            if (wallet.Balance < paymentTransaction.PriceWithTax)
            {
                return;
            }
            wallet.Balance -= paymentTransaction.PriceWithTax;
            toWallet.Balance += paymentTransaction.Price;
            try
            {
                _context.Add(paymentTransaction);
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
    }
}
