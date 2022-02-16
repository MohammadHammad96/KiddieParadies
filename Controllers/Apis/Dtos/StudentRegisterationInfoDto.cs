using KiddieParadies.CustomValidations;
using System;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class StudentRegistrationInfoDto
    {
        public int Id { get; set; }

        [Display(Name = "العام الدراسي")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "هذا الحقل إجباري")]
        [RequireTypeahead("YearId")]
        public string YearName { get; set; }

        public int YearId { get; set; }

        [Display(Name = "من تاريخ")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public DateTime FromDate { get; set; }

        [Display(Name = "حتى تاريخ")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [LessThanDateTime("FromDate")]
        public DateTime ToDate { get; set; }

        [Display(Name = "المرحلة الأولى")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد الطلاب يجب أن يكون بين {1} و {2}.")]
        public int LevelOne { get; set; }

        [Display(Name = "المرحلة الثانية")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد الطلاب يجب أن يكون بين {1} و {2}.")]
        public int LevelTwo { get; set; }

        [Display(Name = "المرحلة الثالثة")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد الطلاب يجب أن يكون بين {1} و {2}.")]
        public int LevelThree { get; set; }
    }
}