using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class WithdrawFundsDTO
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
