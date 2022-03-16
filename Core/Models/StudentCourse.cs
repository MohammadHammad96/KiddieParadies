namespace KiddieParadies.Core.Models
{
    public class StudentCourse : Entity
    {
        public int StudentId { get; set; }

        public YearStudent Student { get; set; }

        public int CourseId { get; set; }

        public CourseClassRoom Course { get; set; }
    }
}