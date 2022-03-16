using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KiddieParadies.Core.Models
{
    public class Message : Entity
    {
        public int SenderId { get; set; }

        public ApplicationUser Sender { get; set; }

        public int ReceiverId { get; set; }

        public ApplicationUser Receiver { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Time { get; set; }

        public bool IsRead { get; set; }
    }
}
