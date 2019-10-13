using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyEmailService.Models
{
    public enum MessageState
    {
        Unread,
        Read,
        Deleted
    }

    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public DateTime TimeSent { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string FromUser { get; set; }

        [Required]
        public string ToUser { get; set; }

        public MessageState MessageState { get; set; }

        public void read()
        {
            //MessageState = MessageStates.
        }
    }
}
