using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.CustomValidations
{
    public class RequireHtmlEditorAttribute : ValidationAttribute, IClientModelValidator
    {
        public new string ErrorMessage => "هيكل الصفحة إلزامي";
        public RequireHtmlEditorAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var columnValue = value.ToString();
            if (string.IsNullOrWhiteSpace(columnValue))
                return new ValidationResult(ErrorMessage);
                
            return ValidationResult.Success;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requirehtmleditor", ErrorMessage);
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