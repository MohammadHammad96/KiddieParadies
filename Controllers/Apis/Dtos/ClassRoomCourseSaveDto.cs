using System;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class ClassRoomCourseSaveDto
    {
        public int ClassRoom { get; set; }

        public DayOfWeek Day { get; set; }

        public int Order { get; set; }

        public int TeacherId { get; set; }

        public int CourseId { get; set; }
    }
}