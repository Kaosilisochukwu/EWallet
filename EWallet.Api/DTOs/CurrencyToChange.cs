using EWallet.Api.Validations;
using System.ComponentModel.DataAnnotations;

namespace EWallet.Api.DTOs
{
    public class CurrencyToChange
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Currency is required")]
        [ValidateCurrencies(ErrorMessage = "Currency is not supported")]
        public string NewCurrency { get; set; }
    }
}
