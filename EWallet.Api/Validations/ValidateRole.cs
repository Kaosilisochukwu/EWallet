using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Validations
{
    public class ValidateRole : ValidationAttribute
    {
        protected override ValidationResult
               IsValid(object value, ValidationContext validationContext)
        {
            var roles = new List<string> { "Noob", "Elite", "Admin" };
            if (roles.Contains(value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult
                    ("Role type not allowed");
            }
        }
    }
}
