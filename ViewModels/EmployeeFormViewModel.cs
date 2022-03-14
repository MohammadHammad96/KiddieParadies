using KiddieParadies.CustomValidations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [MaxLength(50, ErrorMessage = "عدد الأحرف الأقصى يجب أن يكون {1}")]
        [DisplayName("الاسم")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [MaxLength(50, ErrorMessage = "عدد الأحرف الأقصى يجب أن يكون {1}")]
        [DisplayName("اللقب")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("تاريخ الميلاد")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [DisplayName("الصورة")]
        [RequiredFile("Id")]
        [FileType("jpeg", "jpg", "png")]
        public IFormFile Image { get; set; }

        [DisplayName("السيرة الذاتية")]
        [RequiredFile("Id", ErrorMessage = "الملف إجباري")]
        [FileType("pdf", "doc", "docx")]
        public IFormFile Resume { get; set; }

        [DisplayName("الجنس")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public bool IsMale { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("ملخص عن الخبرة")]
        [MinLength(100, ErrorMessage = "عدد الأحرف الأدنى يجب أن يكون {1}")]
        [MaxLength(500, ErrorMessage = "عدد الأحرف الأقصى يجب أن يكون {1}")]
        public string ExperienceSummary { get; set; }

        [DisplayName("الصفة")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string Role { get; set; }

        public ICollection<IFormFile> Certificates { get; set; }

        public Dictionary<string, string> Roles { get; set; }

        public EmployeeFormViewModel()
        {
            Certificates = new List<IFormFile>();
            Roles = new Dictionary<string, string>();
            Roles.Add("Teacher", "معلم");
            Roles.Add("Driver", "سائق");
            Roles.Add("Escort", "مرافق سائق");
        }
    }
}
