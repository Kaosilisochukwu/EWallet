using EWallet.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Model
{
    public class Fund
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Wallet Id is required")]
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        [Required(ErrorMessage = "Funding Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Field must have 3 characters")]
        [ValidateCurrencies(ErrorMessage = "Currency is not supported")]
        public string FundingCurrency { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [ValidateAmount(ErrorMessage = "Amount can not be less than 1")]
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ApprovedDate { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [ForeignKey("User")]
        public string ApprovedBy { get; set; }
        public User User { get; set; }
    }
}
