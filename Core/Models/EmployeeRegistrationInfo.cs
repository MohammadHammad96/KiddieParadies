using System;

namespace KiddieParadies.Core.Models
{
    public class EmployeeRegistrationInfo
    {
        public int Id { get; set; }
        public int YearId { get; set; }

        public Year Year { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Teacher { get; set; }

        public int Driver { get; set; }

        public int Escort { get; set; }
    }
}