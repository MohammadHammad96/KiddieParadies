using KiddieParadies.Core.Models;
using System.Collections.Generic;

namespace KiddieParadies.ViewModels
{
    public class TripViewModel
    {
        public ICollection<Trip> Trips { get; set; }
        public ICollection<YearEmployee> Drivers { get; set; }
        public ICollection<YearEmployee> Escorts { get; set; }
    }
}