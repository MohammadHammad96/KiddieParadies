using System;

namespace KiddieParadies.Core.Models
{
    public class Attendance : Entity
    {
        public DateTime Time { get; set; }

        public bool IsAttend { get; set; }

        public int TripId { get; set; }

        public Trip Trip { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}