using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class YearStudent
    {
        public int Id { get; set; }

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
    }
}