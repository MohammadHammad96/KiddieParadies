using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public abstract class BasePage : Entity
    {
        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public string MainImageName { get; set; }
    }
}