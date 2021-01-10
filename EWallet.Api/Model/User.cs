using Microsoft.AspNetCore.Identity;
using System;

namespace EWallet.Api.Model
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
