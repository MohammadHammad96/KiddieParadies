using System;

namespace KiddieParadies.Core.Models
{
    public class StudentRegistrationInfo : Entity
    {
        public int YearId { get; set; }

        public Year Year { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int LevelOne { get; set; }

        public int LevelTwo { get; set; }

        public int LevelThree { get; set; }
    }
}