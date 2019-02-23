using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DrrrAsync;
using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{
    /// <summary>
    /// The Context object is used to make writing bots easier by providing relevant information using it's properties.
    /// </summary>
    public class Context : DrrrMessage
    {
        public DrrrBot Client { get; private set; }

        public Context(DrrrBot client, DrrrMessage message)
        {
            ID = message.ID;
            Text = message.Text;
            Content = message.Content;
            Url = message.Url;

            Secret = message.Secret;

            Type = message.Type;
            Room = message.Room;
            Timestamp = message.Timestamp;
            Author = message.Author;
            Target = message.Target;

            Client = client;
        }
        
        /// <param name="Message">The message you want to send</param>
        /// <param name="Direct">Whether or not you want the mssage to be a direct message. Default: false</param>
        public async Task Respond(string Message, string Url = "", bool Direct = false) =>
            await Client.SendMessage(Message, Url, (Direct) ? Author.Name : null);
    }
}