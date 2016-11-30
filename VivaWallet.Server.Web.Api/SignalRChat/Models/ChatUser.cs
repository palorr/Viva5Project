using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class ChatUser
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }
}