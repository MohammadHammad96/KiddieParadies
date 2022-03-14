using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class Employee : Entity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }

        public bool IsMale { get; set; }

        [Required]
        [MinLength(100)]
        [MaxLength(500)]
        public string ExperienceSummary { get; set; }

        [Required]
        public string ImageName { get; set; }

        [Required]
        public string ResumeName { get; set; }

        public bool IsValid { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ICollection<Certificate> Certificates { get; set; }

        public ICollection<YearEmployee> EmployeeYears { get; set; }

        public Employee()
        {
            Certificates = new List<Certificate>();
            EmployeeYears = new List<YearEmployee>();
        }
    }
}
