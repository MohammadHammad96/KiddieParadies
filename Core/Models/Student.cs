using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class Student
    {
        public int Id { get; set; }

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
    }
}