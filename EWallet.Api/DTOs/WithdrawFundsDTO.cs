using EWallet.Api.Validations;
using System.ComponentModel.DataAnnotations;

namespace EWallet.Api.DTOs
{
    public class WithdrawFundsDTO
    {
        [Required(ErrorMessage = "Wallet Id is required")]
        public int WalletId { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        [ValidateAmount]
        public decimal Amount { get; set; }
    }
}
