using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Course : Entity
    {
        [Required]
        public string Name { get; set; }

        public ICollection<LevelCourse> CourseLevels { get; set; }

        public Course()
        {
            CourseLevels = new List<LevelCourse>();
        }
    }
}