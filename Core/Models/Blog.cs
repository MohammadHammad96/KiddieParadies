using System.ComponentModel.DataAnnotations;
using System;

namespace KiddieParadies.Core.Models
{
    public class Blog : BasePage
    {
        public DateTime Time { get; set; }

        [Required]
        [MaxLength(250)]
        public string ShortContent { get; set; }

        public int? YearId { get; set; }

        public Year Year { get; set; }
    }
}