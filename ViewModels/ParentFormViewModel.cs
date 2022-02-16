using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
        public string FatherLaseName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("اسم الأم الكامل")]
        public string MotherFullName { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [DisplayName("صورة صفحة الأب بدفتر العائلة")]
        public IFormFile FatherIdentityImage { get; set; }

        [DisplayName("صورة صفحة الأم بدفتر العائلة")]
        public IFormFile MotherIdentityImage { get; set; }
    }
}