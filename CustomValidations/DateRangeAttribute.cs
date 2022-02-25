using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
            var minData = min.Split("/");
            var minInt = new List<int>();
            foreach (var data in minData)
                minInt.Add(int.Parse(data));
            _min = new DateTime(minInt[2], minInt[1], minInt[0]);
            var maxData = max.Split("/");
            var maxInt = new List<int>();
            foreach (var data in maxData)
                maxInt.Add(int.Parse(data));
            _max = new DateTime(maxInt[2], maxInt[1], maxInt[0]);
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