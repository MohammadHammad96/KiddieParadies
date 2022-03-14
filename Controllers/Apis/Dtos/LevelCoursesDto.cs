using KiddieParadies.Core.Helpers;
using System.Collections.Generic;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class LevelCoursesDto
    {
        public ICollection<LevelCourseHelper> LevelCourses { get; set; }

        public LevelCoursesDto()
        {
            LevelCourses = new List<LevelCourseHelper>();
        }
    }
}