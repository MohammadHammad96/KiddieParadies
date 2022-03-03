using KiddieParadies.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.CustomValidations
{
    public class RequiredFileAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _classIdProperty;

        public RequiredFileAttribute(string classIdProperty)
        {
            if (string.IsNullOrWhiteSpace(classIdProperty))
                throw new ArgumentException("This parameter shouldn't be null or whitespace",
                    "classIdProperty");

            _classIdProperty = classIdProperty;

            if (string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = "الصورة إجبارية";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file != null && file.Length > 0)
                return ValidationResult.Success;

            var propertyInfo = validationContext.ObjectType.GetProperty(_classIdProperty);
            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (propertyValue == null)
                return new ValidationResult(ErrorMessage);

            return propertyValue.ToInt() > 0 ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = "الصورة إجبارية";

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requireimage", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-requireimage-classidproperty", _classIdProperty);
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
                return;

            attributes.Add(key, value);
        }
    }
}