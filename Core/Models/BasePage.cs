using System.ComponentModel.DataAnnotations;
using KiddieParadies.CustomValidations;

namespace KiddieParadies.Core.Models
{
    public abstract class BasePage
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public string MainImageName { get; set; }
    }
}