using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Viva.Wallet.BAL.Repository;
using System.Collections.Generic;

namespace NavigationBot
{
    [BotAuthentication]
    [RoutePrefix("api/messages")]
    public class MessagesController : ApiController
    {
        // OK
        [AllowAnonymous]
        [HttpGet]
        [Route("getAvailableOptions")]
        public HttpResponseMessage GetAvailableOptions()
        {
            IList<string> availableOptions = new List<string>();
            availableOptions.Add("1. Get Number of Projects Created");
            availableOptions.Add("2. Get Number of Users Registered");
            availableOptions.Add("3. Navigate to Project Create Page");

            var response = Request.CreateResponse(HttpStatusCode.OK, availableOptions);
            return response;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //THIS IS THE TEXT THE USER HAS SENT TO THE BOT
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                IList<string> availableOptions = new List<string>();
                availableOptions.Add("1. Get Number of Projects Created");
                availableOptions.Add("2. Get Number of Users Registered");
                availableOptions.Add("3. Navigate to Project Create Page");

                // return our reply to the user
                //Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                string str = "Available Options: ";
                for (int i=0; i<availableOptions.Count; i++)
                {
                    str += availableOptions[i];
                }
                Activity reply = activity.CreateReply(str);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            //SYSTEM MESSAGES LIKE A USER LEFT THE CONVERSATION OR THE CONVERSATION WITH BOT ENDED ETC.
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        // FOR HANDLING SYSTEM MESSAGES FROM ABOVE
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }

            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }

            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }

            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }

            else if (message.Type == ActivityTypes.Ping)
            {
            }

            else if (message.Type == "BotAddedToConversation")
            {
                return message.CreateReply("Hello Navigation Botty face!");
            }

            return null;
        }
    }
}