using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Model
{
    public class Fund
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public string FundingCurrency { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime ApprovedDate { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [ForeignKey("User")]
        public string ApprovedBy { get; set; }
        public User User { get; set; }
    }
}
