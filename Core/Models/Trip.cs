using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Trip : Entity
    {
        [DataType(DataType.Time)]
        //[Column(TypeName = "Time")]
        //[NotMapped]
        public DateTime Time { get; set; }

        public bool Type { get; set; }

        [DefaultValue(false)]
        public bool IsActive { get; set; }

        public int DriverId { get; set; }

        public YearEmployee Driver { get; set; }

        public int EscortId { get; set; }

        public YearEmployee Escort { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

        public ICollection<Student> Students { get; set; }

        public Trip()
        {
            Attendances = new List<Attendance>();
            Students = new List<Student>();
        }
    }
}
