using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiddieParadies.Core.Models
{
    public class Notification : Entity
    {
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public bool IsRead { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [Column(TypeName = "varchar(MAX)")]
        public string Url { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
