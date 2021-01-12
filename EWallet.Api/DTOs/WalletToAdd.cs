using EWallet.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class WalletToAdd
    {
        [Required(ErrorMessage = "Wallet currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be three characters")]
        public string Currency { get; set; }

        public decimal Amount { get; set; }
    }
}
