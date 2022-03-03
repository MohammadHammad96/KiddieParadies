using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.CustomValidations
{
    public class RequireTypeaheadAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string[] _otherProperties;

        private new string ErrorMessage =>
            $"الرجاء اختيار عام من القائمة دون التعديل عليه.";

        public RequireTypeaheadAttribute(params string[] otherProperties)
        {
            if (otherProperties.Length == 0) // would not make sense
            {
                throw new ArgumentException("At least one other property name must be provided");
            }
            _otherProperties = otherProperties;
            //ErrorMessage = _DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            foreach (string property in _otherProperties)
            {
                var propertyName = validationContext.ObjectType.GetProperty(property);
                if (propertyName == null)
                {
                    continue;
                }
                var propertyValue = propertyName.GetValue(validationContext.ObjectInstance, null);
                if (propertyValue == null)
                {
                    return new ValidationResult(ErrorMessage);
                }

                var val = propertyValue.ToString();
                if (string.IsNullOrWhiteSpace(val))
                    return new ValidationResult(ErrorMessage);

                if (int.Parse(val) == 0)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiretypeahead", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-requiretypeahead-otherproperties", string.Join(",", _otherProperties));
        }

        private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return;
            }

            attributes.Add(key, value);
        }
    }
}