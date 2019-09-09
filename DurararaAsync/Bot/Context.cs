using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DrrrBot.Objects;

namespace DrrrBot.Core
{
    public class Context : DrrrMessage
    {


        public Bot Client { get; private set; }

        public Context(Bot aClient, DrrrMessage aMessage)
        {
            ID = aMessage.ID;
            Text = aMessage.Text;
            Content = aMessage.Content;
            Url = aMessage.Url;

            Secret = aMessage.Secret;

            Type = aMessage.Type;
            Room = aMessage.Room;
            Timestamp = aMessage.Timestamp;
            Author = aMessage.Author;
            Target = aMessage.Target;

            Client = aClient;
        }


        /// <summary>
        /// RespondAsync wraps SendMessage and SendDirectMessage helping keep bot code more readable.
        /// </summary>
        /// <param name="Message">The message you want to send</param>
        /// <param name="Direct">Whether or not you want the mssage to be a direct message. Default: false</param>
        public async Task RespondAsync(string Message, string Url = "", bool Direct = false) =>
            await Client.SendMessage(Message, (Direct) ? Author.Name : "", Url);

        /// <summary>
        /// Responds to a user with a direct message
        /// </summary>
        /// <param name="Message">The message you want to send</param>
        public async Task RespondDirect(string Message, string Url = "") =>
            await Client.SendDirectMessage(Message, Author.Name, Url);
    }
}
