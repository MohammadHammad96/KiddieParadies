using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace KiddieParadies.CustomValidations
{
    public class FileTypeAttribute : ValidationAttribute
    {
        private readonly string[] _acceptedFileTypes;

        public FileTypeAttribute(params string[] acceptedFileTypes)
        {
            if (string.IsNullOrWhiteSpace(ErrorMessage))
                ErrorMessage = "لاحقة الملف يجب أن تكون " + string.Join(" أو ", acceptedFileTypes);

            for (var i = 0; i < acceptedFileTypes.Length; i++)
                acceptedFileTypes[i] = string.Concat(".", acceptedFileTypes[i]);

            _acceptedFileTypes = acceptedFileTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file == null || file.Length <= 0)
                return ValidationResult.Success;

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (_acceptedFileTypes.Any(t => t == fileExtension))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}