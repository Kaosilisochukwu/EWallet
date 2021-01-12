using EWallet.Api.Data;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Interfaces;
using EWallet.Api.Utils;
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
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == model.UserId);
            if(user != null)
            {
                if(user.Role == "Elite" || user.Role == "Noob" && !_context.Wallets.Any(wallet => wallet.UserId == user.Id))
                {
                    if (!_context.Wallets.Any(wallet => wallet.UserId == user.Id))
                        model.IsMainCurrency = true;
                    await _context.Wallets.AddAsync(model);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ChangeMainCurrency(string currency, string userId)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id == userId);
            if(user.Role == "Elite")
            {
                var mainWallet = _context.Wallets.FirstOrDefault(wallet => wallet.UserId == userId && wallet.IsMainCurrency);
                var newMainWallet = _context.Wallets.FirstOrDefault(wallet => wallet.Currency == currency);
                mainWallet.IsMainCurrency = false;
                newMainWallet.IsMainCurrency = true;
                _context.Wallets.AddRange(new List<Wallet> { mainWallet, newMainWallet });
            }
            return await _context.SaveChangesAsync();
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


        public async Task<Wallet> GetMainWallet(string userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.IsMainCurrency && wallet.UserId == userId);
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
