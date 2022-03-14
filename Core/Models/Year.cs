using KiddieParadies.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class Year : Entity
    {
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Display(Name = "من تاريخ")]
        [DateRange("01/01/2022", "31/12/2040")]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [Display(Name = "حتى تاريخ")]
        [DateRange("01/01/2023", "31/12/2041")]
        [LessThanDateTime("FromDate")]
        public DateTime ToDate { get; set; }

        public ICollection<StudentRegistrationInfo> StudentRegistrationInfos { get; set; }

        public ICollection<LevelCourse> LevelCourses { get; set; }

        public Year()
        {
            StudentRegistrationInfos = new List<StudentRegistrationInfo>();

            LevelCourses = new List<LevelCourse>();
        }
    }
}