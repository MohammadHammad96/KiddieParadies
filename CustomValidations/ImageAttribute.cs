using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace KiddieParadies.CustomValidations
{
    public class ImageAttribute : ValidationAttribute
    {
        private static readonly string[] AcceptedFileTypes = new string[] { ".jpg", ".jpeg", ".png" };

        public ImageAttribute()
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = "لاحقة الصورة يجب أن تكون jpg أو jpeg أو png";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file == null || file.Length <= 0)
                return ValidationResult.Success;

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (AcceptedFileTypes.Any(t => t == fileExtension))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}