using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Validations
{
    public class ValidateAmount : ValidationAttribute
    {
         protected override ValidationResult 
                IsValid(object value, ValidationContext validationContext)
        {   
            decimal _dateJoin = decimal.Parse(value.ToString()); 
            if (_dateJoin >= 1)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult
                    ("Amount can not be less than 1");
            }
        } 
    }
}
