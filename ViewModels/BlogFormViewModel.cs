using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KiddieParadies.ViewModels
{
    public class BlogFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هيكل صفحة الخبر إجباري")]
        public string Content { get; set; }

        [DisplayName("العنوان")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [MaxLength(250, ErrorMessage = "عدد الحروف يجب ألا يتجاوز {1}")]
        public string Title { get; set; }
        
        [DisplayName("الصورة الرئيسية")]
        public IFormFile Image { get; set; }

        [DisplayName("محتوى مختصر")]
        [Required(ErrorMessage = "هذا الحقل إجباري")]
        [MaxLength(250, ErrorMessage = "عدد الحروف يجب ألا يتجاوز {1}")]
        public string ShortContent { get; set; }
    }
}