using System.Collections.Generic;

namespace KiddieParadies.Core.Models
{
    public class LevelCourse : Entity
    {
        public int YearId { get; set; }

        public Year Year { get; set; }

        public int LevelId { get; set; }

        public Level Level { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public ICollection<CourseClassRoom> CourseClassRooms { get; set; }

        public LevelCourse()
        {
            CourseClassRooms = new List<CourseClassRoom>();
        }
    }
}