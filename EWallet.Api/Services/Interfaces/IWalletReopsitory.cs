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
        public Task<int> AddWAllet(Wallet model);
        public Task<int> UpdateWallet(Wallet model);
        public Task<int> DeleteWallet(int walletId);
        public Task<IEnumerable<Wallet>> GetWallets();
        public Task<IEnumerable<Wallet>> GetAllUserWallet(string userId);
        public Task<Wallet> GetWalletById(int walletId);
        public Task<int> FundWallet(int walletId, decimal amount);
        public Task<int> Withdrawfunds(WithdrawFundsDTO model, string userId);
        public Task<int> ChangeMainCurrency(CurrencyToChange model);
        public Task<int> RequestFunding(Fund model);
        public Task<int> ApproveFundRequest(int requestId, string userId);
        public Task<int> DeclineFundRequest(int requestId);
        public Task<Fund> GetRequestById(int requestId);
        public Task<IEnumerable<Wallet>> GetWalletByCurrency(string currecny);
        public Task<Wallet> GetMainCurrency(string userId);
        public Task<int> AdminFunding(FundRequestDTO model);
    }
}
