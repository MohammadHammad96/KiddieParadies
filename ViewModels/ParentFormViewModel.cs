using KiddieParadies.CustomValidations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.ViewModels
{
    public class ParentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("اسم الأب")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("كنية الأب")]
        public string FatherLastName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("اسم الأم الكامل")]
        public string MotherFullName { get; set; }

        [Required]
        public int UserId { get; set; }

        [RequiredFile("Id")]
        [Image]
        [DisplayName("صورة صفحة الأب بدفتر العائلة")]
        public IFormFile FatherIdentityImage { get; set; }

        [RequiredFile("Id")]
        [Image]
        [DisplayName("صورة صفحة الأم بدفتر العائلة")]
        public IFormFile MotherIdentityImage { get; set; }
    }
}