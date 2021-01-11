using EWallet.Api.Model;
using System;

namespace EWallet.Api.DTOs
{
    public class FundRequestDTO
    {
        public int WalletId { get; set; }
        public string FundingCurrenct { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public RequestStatus Status { get; set; }
    }
}
