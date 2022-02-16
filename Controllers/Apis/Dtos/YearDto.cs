using KiddieParadies.CustomValidations;
using System;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class YearDto
    {
        public int Id { get; set; }

        [Display(Name = "العام الدراسي")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "من تاريخ")]
        [Required]
        [DateRange("01/01/2022", "31/12/2040")]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "حتى تاريخ")]
        [Required]
        [DateRange("01/01/2023", "31/12/2041")]
        public DateTime ToDate { get; set; }
    }
}