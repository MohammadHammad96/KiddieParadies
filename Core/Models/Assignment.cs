using System;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Assignment : Entity
    {
        [Required]
        public string Content { get; set; }

        public DateTime Deadline { get; set; }

        public int CourseId { get; set; }

        public CourseClassRoom Course { get; set; }
    }
}