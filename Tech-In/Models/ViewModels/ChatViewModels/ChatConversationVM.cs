using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tech_In.Models.ViewModels.ChatViewModels
{
    public class ChatConversationVM
    {
        public enum MessageStatus
        {
            Sent,
            Delivered
        }

        public int Id { set; get; }
        public string Message { set; get; }
        public string SenderId { set; get; }
        public string RecieverId { set; get; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { set; get; }
    }
}
