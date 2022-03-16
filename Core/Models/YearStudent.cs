using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class YearStudent : Entity
    {
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? ToDate { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }

        public int YearId { get; set; }

        public Year Year { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }

        public YearStudent()
        {
            StudentCourses = new List<StudentCourse>();
        }
    }
}