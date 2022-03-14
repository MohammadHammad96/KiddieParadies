using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class YearEmployee : Entity
    {
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime? ToDate { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int YearId { get; set; }

        public Year Year { get; set; }
    }
}
