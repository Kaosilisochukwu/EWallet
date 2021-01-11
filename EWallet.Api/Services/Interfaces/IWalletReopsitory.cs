using EWallet.Api.DTOs;
using EWallet.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Services.Interfaces
{
    public interface IWalletReopsitory
    {
        Task<int> AddWAllet(Wallet model);
        Task<int> UpdateWallet(Wallet model);
        Task<int> DeleteWallet(int walletId);
        Task<IEnumerable<Wallet>> GetWallets();
        Task<IEnumerable<Wallet>> GetAllUserWallet(string userId);
        Task<Wallet> GetWalletById(int walletId);
        Task<int> FundWallet(int walletId, decimal amount);
        Task<int> Withdrawfunds(WithdrawFundsDTO model, string userId);
        Task<int> ChangeMainCurrency(string currency, string userId);
    }
}
