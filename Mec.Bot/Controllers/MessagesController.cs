﻿using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using System;
using Mec.Bot.Dialogs;

namespace Mec.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            //Configurar o EndPoint no LUIS
            //var attributes = new LuisModelAttribute(
            //    ConfigurationManager.AppSettings["LuisId"],
            //    ConfigurationManager.AppSettings["LuisSubscriptionKey"]);
            //var service = new LuisService(attributes);

            if (activity.Type == ActivityTypes.Message)
            {
                //Colocar os '...' que está digitando
                var reply = activity.CreateReply();
                reply.Type = ActivityTypes.Typing;
                reply.Text = null;
                await connector.Conversations.ReplyToActivityAsync(reply);

                //await Conversation.SendAsync(activity, () => new Dialogs.MecBotDialog(service));
                await Conversation.SendAsync(activity, () => new Dialogs.MecBotDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }

            //if (activity.Type == ActivityTypes.Message)
            //{
            //    //Colocar os '...' que está digitando
            //    var reply = activity.CreateReply();
            //    reply.Type = ActivityTypes.Typing;
            //    reply.Text = null;
            //    await connector.Conversations.ReplyToActivityAsync(reply);

            //    await Conversation.SendAsync(activity, () => new RootDialog());
            //}
            //else
            //{
            //    HandleSystemMessage(activity);
            //}

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

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

            return null;
        }
    }
}
