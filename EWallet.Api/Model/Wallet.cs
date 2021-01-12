using EWallet.Api.Validations;
using System;
using System.ComponentModel.DataAnnotations;

namespace EWallet.Api.Model
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User Id is required", AllowEmptyStrings = false)]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Funding Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Field must have 3 characters")]
        public string Currency { get; set; }

        [Required]
        [ValidateBalance]
        public decimal Balance { get; set; }
        
        [Required]
        public bool IsMainCurrency { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}