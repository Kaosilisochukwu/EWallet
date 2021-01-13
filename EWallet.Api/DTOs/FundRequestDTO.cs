using EWallet.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace EWallet.Api.DTOs
{
    public class FundRequestDTO
    {
        [Required(ErrorMessage = "Wallet Id is required")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Funding currency is required", AllowEmptyStrings = false)]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Funding currency must be 3 characters")]
        [ValidateCurrencies(ErrorMessage = "Currency is not supported")]
        public string FundingCurrency { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [ValidateAmount(ErrorMessage = "Amount can not be less than 1")]
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}
