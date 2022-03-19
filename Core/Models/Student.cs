using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class Student : Entity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        public string ImageName { get; set; }

        public bool IsMale { get; set; }

        public bool IsValid { get; set; }

        public int ParentId { get; set; }

        public Parent Parent { get; set; }

        public int Level { get; set; }

        public int ClassRoom { get; set; }

        public int TripId { get; set; }

        public Trip Trip { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<YearStudent> YearStudents { get; set; }

        public Student()
        {
            YearStudents = new List<YearStudent>();
            //Attendances = new List<Attendance>();
        }
    }
}