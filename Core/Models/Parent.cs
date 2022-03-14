using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Parent : Entity
    {
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherLastName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string MotherFullName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherIdentityImageName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string MotherIdentityImageName { get; set; }

        public bool IsValid { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public Location HomeLocation { get; set; }
    }
}