using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class UserToLoginDTO
    {
        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password field is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
