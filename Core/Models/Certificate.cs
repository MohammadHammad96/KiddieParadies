using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Certificate : Entity
    {
        [Required]
        public string ImageName { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}