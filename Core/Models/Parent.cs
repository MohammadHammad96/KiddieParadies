using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Core.Models
{
    public class Parent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherLaseName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string MotherFullName { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string FatherIdentityImagePath { get; set; }

        [Required(ErrorMessage = "هذا الحقل إجباري")]
        public string MotherIdentityImagePath { get; set; }

        public int UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}