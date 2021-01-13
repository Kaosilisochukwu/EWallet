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

        public async Task<int> ChangeMainCurrency(CurrencyToChange model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == model.UserId);
            var mainWallet = _context.Wallets.FirstOrDefault(wallet => wallet.UserId == model.UserId && wallet.IsMainCurrency);
            if(user.Role == "Noob")
            {
                var newBalance = await Currency.ConvertCurrency(mainWallet.Currency, model.NewCurrency, mainWallet.Balance);
                mainWallet.Balance = newBalance;
                _context.Wallets.Update(mainWallet);
                return await _context.SaveChangesAsync();
            }
            else if(user.Role == "Elite")
            {
                var newMainWallet = _context.Wallets.FirstOrDefault(wallet => wallet.UserId == model.UserId && wallet.IsMainCurrency);
                if(mainWallet != null)
                {
                    mainWallet.IsMainCurrency = false;
                    newMainWallet.IsMainCurrency = true;
                    _context.Wallets.UpdateRange(new List<Wallet> { mainWallet, newMainWallet });
                }
                else
                {
                    _context.Wallets.Add(new Wallet { IsMainCurrency = true, Currency = model.NewCurrency, UserId = model.UserId });
                }    
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
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            var wallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == model.WalletId && wallet.UserId == userId);
            if(wallet != null && wallet.Balance >= model.Amount)
            {
                wallet.Balance -= model.Amount;
                _context.Wallets.Update(wallet);
            }
            else if(wallet != null && !wallet.IsMainCurrency)
            {
                var mainWallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.UserId == userId && wallet.IsMainCurrency);
                var mainCurrencyToWithdrawalCurrency = await Currency.ConvertCurrency(mainWallet.Currency, wallet.Currency,  model.Amount - wallet.Balance);
                if(model.Amount <= wallet.Balance + mainCurrencyToWithdrawalCurrency)
                {
                    model.Amount -= wallet.Balance;
                    wallet.Balance = 0;
                    var convertedRemainder = await Currency.ConvertCurrency(wallet.Currency, mainWallet.Currency, model.Amount);
                    mainWallet.Balance -= convertedRemainder;
                    _context.Wallets.UpdateRange(new List<Wallet> { wallet, mainWallet });
                }
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> ApproveFundRequest(int requestId, string userId)
        {
            var request = await _context.FundRequests.FirstOrDefaultAsync(fundRequest => fundRequest.Id == requestId);
            if (request != null)
            {
                var fundResult = await FundWallet(request.WalletId, request.Amount);
                if (fundResult > 0)
                {
                    request.Status = RequestStatus.Approved;
                    request.ApprovedBy = userId;
                    request.ApprovedDate = DateTime.Now;
                    _context.FundRequests.Update(request);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeclineFundRequest(int requestId)
        {
            var request = await _context.FundRequests.FirstOrDefaultAsync(fundRequest => fundRequest.Id == requestId);
            if (request != null)
            {
                request.Status = RequestStatus.Declined;
                _context.FundRequests.Update(request);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RequestFunding(Fund model)
        {
            await _context.FundRequests.AddAsync(model);
            return await _context.SaveChangesAsync();
        }


        public async Task<Fund> GetRequestById(int requestId)
        {
            return await _context.FundRequests.FirstOrDefaultAsync(request => request.Id == requestId);
        }

        public async Task<IEnumerable<Fund>> GetWalleRequests()
        {
            return await _context.FundRequests.ToListAsync();
        }
        public async Task<Fund> GetWalleRequestById(int id)
        {
            return await _context.FundRequests.FirstOrDefaultAsync(fund => fund.Id == id);
        }

        public async Task<IEnumerable<Wallet>> GetWalletByCurrency(string currecny)
        {
            return await _context.Wallets.Where(wallet => wallet.Currency == currecny).ToListAsync();
        }

        public async Task<IEnumerable<Wallet>> GetWalletByUserId(string userId)
        {
            return await _context.Wallets.Where(wallet => wallet.UserId == userId).ToListAsync();
        }

        public async Task<Wallet> GetWalletByCurrencyAndUserId(string currecny, string userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Currency == currecny && wallet.UserId == userId);
        }

        public async Task<Wallet> GetMainCurrency(string userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.UserId == userId && wallet.IsMainCurrency);
        }

        public async Task<int> AdminFunding(FundRequestDTO model)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.Id == model.WalletId);
            if(wallet != null)
            {
                if (model.FundingCurrency != wallet.Currency)
                    wallet.Balance += await Currency.ConvertCurrency(model.FundingCurrency, wallet.Currency, model.Amount);
                else
                    wallet.Balance += model.Amount;
                _context.Wallets.Update(wallet);
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> PromoteNoob(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            user.Role = "Elite";
            _context.Users.Update(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DemoteElite(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            user.Role = "Noob";
            var eliteMainWallet = await GetMainCurrency(userId);
            var eliteWallets = await _context.Wallets.Where(wallet => wallet.UserId == userId).ToListAsync();
            foreach (var wallet in eliteWallets)
            {
                if(wallet.Id != eliteMainWallet.Id)
                {
                    eliteMainWallet.Balance += await Currency.ConvertCurrency(wallet.Currency, eliteMainWallet.Currency, wallet.Balance);
                    _context.Wallets.Remove(wallet);
                }
            }
            _context.UpdateRange(new List<object> { eliteMainWallet, user });
            return await _context.SaveChangesAsync();
        }
    }
}
