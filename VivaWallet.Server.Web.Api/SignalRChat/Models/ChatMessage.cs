using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class ChatMessage
    {
        public long FromId { get; set; }
        public string FromName { get; set; }
        public string Message { get; set; }
        public DateTime Sent { get; set; }
    }
}