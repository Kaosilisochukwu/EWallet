using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.DTOs
{
    public class UserToDeleteDTO
    {
        [Required]
        public string UserId { get; set; }
    }
}
