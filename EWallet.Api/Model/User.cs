using EWallet.Api.Validations;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace EWallet.Api.Model
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Firstname is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Firstname must be greater that and less than 200")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Lastname must be greater that and less than 200")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User role is required")]
        [ValidateRole(ErrorMessage = "This role type is not allwoed")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Date Created is required")]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
