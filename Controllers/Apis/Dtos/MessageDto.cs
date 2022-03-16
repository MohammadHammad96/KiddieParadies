using System;
using System.ComponentModel.DataAnnotations;

namespace KiddieParadies.Controllers.Apis.Dtos
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool IsRead { get; set; }
    }
}