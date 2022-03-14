using KiddieParadies.Core.Helpers;
using KiddieParadies.Core.Models;
using System.Collections.Generic;

namespace KiddieParadies.ViewModels
{
    public class LevelCourseViewModel
    {
        public ICollection<Level> Levels { get; set; }

        public ICollection<Course> Courses { get; set; }

        public ICollection<LevelCourseHelper> LevelCourses { get; set; }

        public LevelCourseViewModel()
        {
            Levels = new List<Level>();
            Courses = new List<Course>();
            LevelCourses = new List<LevelCourseHelper>();
        }
    }
}