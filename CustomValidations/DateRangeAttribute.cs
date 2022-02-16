using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.CustomValidations
{
    public class DateRangeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly DateTime _min;
        private readonly DateTime _max;

        private new string ErrorMessage =>
            $"التاريخ يجب أن يكون بين {_min.ToString("dd-MM-yyyy")} و {_max.ToString("dd-MM-yyyy")}";

        public DateRangeAttribute(string min, string max)
        {
            _min = DateTime.Parse(min);
            _max = DateTime.Parse(max);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;
            if ((date >= _min) && (date <= _max))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-daterange", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-daterange-min", _min.ToString("MM/dd/yyyy"));
            MergeAttribute(context.Attributes, "data-val-daterange-max", _max.ToString("MM/dd/yyyy"));
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