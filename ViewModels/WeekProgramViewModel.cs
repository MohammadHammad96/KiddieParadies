using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.ViewModels
{
    public class WeekProgramFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "المستوى إجباري")]
        public int LevelId { get; set; }

        [Required(ErrorMessage = "رقم الشعبة إجباري")]
        [Range(1, 20, ErrorMessage = "رقم الشعبة يجب أن يكون {1} على الأقل.")]
        public int ClassNumber { get; set; }
    }
}
