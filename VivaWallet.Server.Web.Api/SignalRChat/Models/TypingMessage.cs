using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class TypingMessage
    {
        public string FromName { get; set; }
        public string Message { get; set; }
    }
}