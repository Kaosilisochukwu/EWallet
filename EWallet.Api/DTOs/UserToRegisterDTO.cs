using EWallet.Api.Model;
using EWallet.Api.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class UserToRegisterDTO
    {
        [Required(ErrorMessage = "Firstname is required", AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Firstname must be greater that and less than 200")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required", AllowEmptyStrings = false)]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Lastname must be greater that and less than 200")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "User role is required")]
        [ValidateRole(ErrorMessage = "This role type is not allwoed")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Username is Required", AllowEmptyStrings = false)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must contain only letters and numbers")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be greater or equal to 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password Field is required")]
        [Compare("Password", ErrorMessage = "ConfirmPassword must match  the password field")]
        public string ConfirmPassowrd { get; set; }
    }
}
