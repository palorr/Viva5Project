using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat.Models;
using System.Collections.Generic;

namespace SignalRChat
{
    [HubName("chat")]
    public class ChatHub : Hub
    {
        //NOT USED
        private readonly static IList<ChatUser> listOfChatUsers = new List<ChatUser>();

        public void SendMessage(ChatMessage chatMessage)
        {
            Clients.All.SendMessage(chatMessage);
        }

        public void TypeMessage(TypingMessage typeMessage)
        {
            Clients.All.TypeMessage(typeMessage);
        }

        //NOT USED
        public void NewChatUserAdded(ChatUser chatUser)
        {
            ChatHub.listOfChatUsers.Add(chatUser);

            Clients.All.NewChatUserAdded(listOfChatUsers);
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }

}