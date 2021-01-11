using EWallet.Api.Data;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Services.Repositories
{
    public class WalletRepository : IWalletReopsitory
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddWAllet(Wallet model)
        {
            await _context.Wallets.AddAsync(model);
            return await _context.SaveChangesAsync();
        }

        public Task<int> ChangeMainCurrency(string currency, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteWallet(int walletId)
        {
            var wallet =  await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == walletId);
            _context.Wallets.Remove(wallet);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> FundWallet(int walletId, decimal amount)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == walletId);
            wallet.Balance += amount;
            _context.Wallets.Update(wallet);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Wallet>> GetAllUserWallet(string userId)
        {
            return await _context.Wallets.Where(wallet => wallet.UserId == userId).ToListAsync();
        }

        public async Task<Wallet> GetWalletById(int walletId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == walletId);
        }

        public async Task<IEnumerable<Wallet>> GetWallets()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<int> UpdateWallet(Wallet model)
        {
            _context.Wallets.Update(model);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Withdrawfunds(WithdrawFundsDTO model, string userId)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == model.WalletId && wallet.UserId == userId);
            if(wallet != null && wallet.Balance >= model.Amount)
            {
                wallet.Balance -= model.Amount;
                _context.Wallets.Update(wallet);
            }
            else if(wallet != null && !wallet.IsMainCurrency)
            {
                var mainWallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.UserId == userId && wallet.IsMainCurrency);
                if(model.Amount <= wallet.Balance + mainWallet.Balance)
                {
                    model.Amount -= wallet.Balance;
                    wallet.Balance = 0;
                    mainWallet.Balance -= model.Amount;
                    _context.Wallets.UpdateRange(new List<Wallet> { wallet, mainWallet });
                }
            }
            return await _context.SaveChangesAsync();
        }
    }
}
