using KiddieParadies.CustomValidations;
using System;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class EmployeeRegistrationInfoDto
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

        [Display(Name = "المدرسين")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد المدرسين يجب أن يكون بين {1} و {2}.")]
        public int Teacher { get; set; }

        [Display(Name = "السائقين")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد السائقين يجب أن يكون بين {1} و {2}.")]
        public int Driver { get; set; }

        [Display(Name = "المرافقين")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Range(0, 1000, ErrorMessage = "عدد مرافقين السائقين يجب أن يكون بين {1} و {2}.")]
        public int Escort { get; set; }
    }
}