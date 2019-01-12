using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.Database
{
    public class Conversation
    {
        public Conversation()
        {
            Status = MessageStatus.Sent;
        }

        public enum MessageStatus
        {
            Sent,
            Delivered
        }
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public string SenderId { get; set; }

        public string RecieverId { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        //ApNetUser
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
