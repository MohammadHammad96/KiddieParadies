using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Level : Entity
    {
        public int Order { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<LevelCourse> LevelCourses { get; set; }

        public Level()
        {
            LevelCourses = new List<LevelCourse>();
        }
    }
}
