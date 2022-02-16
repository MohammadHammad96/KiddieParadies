using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace KiddieParadies.CustomValidations
{
    public class PasswordAttribute : ValidationAttribute, IClientModelValidator
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value.ToString();
            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult("كلمة السر إجبارية");

            ErrorMessage = "كلمة السر";
            if (!password.ToCharArray().Any(char.IsDigit))
                ErrorMessage += " يجب أن تحتوي على الأقل";

            return ValidationResult.Success;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-password", ErrorMessage);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}