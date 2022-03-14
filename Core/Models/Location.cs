using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Location : Entity
    {
        [Required(AllowEmptyStrings = false)]
        public string Longitude { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Latitude { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Zoom { get; set; }

        public int ParentId { get; set; }

        public Parent Parent { get; set; }
    }
}