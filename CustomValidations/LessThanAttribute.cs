using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace KiddieParadies.CustomValidations
{
    public class LessThanDateTimeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string[] _otherProperties;
    private const string _DefaultErrorMessage = "The {0} field is required";

        private new string ErrorMessage =>
            $"التاريخ يجب أن يكون أكبر من حقل 'من تاريخ'.";

        public LessThanDateTimeAttribute(params string[] otherProperties)
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
                    else if (DateTime.Parse(propertyValue.ToString()) >= DateTime.Parse(value.ToString()))
                        return new ValidationResult(ErrorMessage);
                }

            return ValidationResult.Success;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-lessthandatetime", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-lessthandatetime-otherproperties", string.Join(",", _otherProperties));
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