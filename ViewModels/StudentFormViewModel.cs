using KiddieParadies.CustomValidations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.ViewModels
{
    public class StudentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [MaxLength(50, ErrorMessage = "عدد الأحرف الأقصى يجب أن يكون {1}")]
        [DisplayName("الاسم")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [DisplayName("تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DisplayName("الصورة")]
        [RequiredFile("Id")]
        [FileType("jpeg", "jpg", "png")]
        public IFormFile Image { get; set; }

        [DisplayName("الجنس")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public bool IsMale { get; set; }

        public int ParentId { get; set; }

        public ICollection<KeyValuePair<int, string>> AvailableLevels { get; set; }

        [DisplayName("المستوى")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public bool? Level { get; set; }

        public StudentFormViewModel()
        {
            AvailableLevels = new List<KeyValuePair<int, string>>();
        }
    }
}