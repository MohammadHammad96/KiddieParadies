using System;
using System.Collections.Generic;

namespace KiddieParadies.Core.Models
{
    public class CourseClassRoom : Entity
    {
        public int ClassRoom { get; set; }

        public DayOfWeek Day { get; set; }

        public int Order { get; set; }

        public int CourseId { get; set; }

        public LevelCourse Course { get; set; }

        public int TeacherId { get; set; }

        public YearEmployee Teacher { get; set; }

        public ICollection<Assignment> Assignments { get; set; }

        public ICollection<StudentCourse> CourseStudents { get; set; }

        public CourseClassRoom()
        {
            Assignments = new List<Assignment>();

            CourseStudents = new List<StudentCourse>();
        }
    }
}
