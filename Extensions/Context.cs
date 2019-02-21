using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DrrrAsync;
using DrrrAsync.Objects;

namespace DrrrAsync
{
    namespace Extensions
    {
        /// <summary>
        /// The Context object is used to make writing bots easier by providing relevant information using it's properties.
        /// </summary>
        public class Context
        {
            public DrrrMessage Message { get; private set; }
            public DrrrUser Author { get; private set; }
            public DrrrRoom Room { get; private set; }

            public Bot Client { get; private set; }

            public Context(Bot aClient, DrrrMessage aMessage, DrrrUser aAuthor, DrrrRoom aRoom)
            {
                Client = aClient;
                Message = aMessage;
                Author = aAuthor;
                Room = aRoom;
            }

            /// <summary>
            /// RespondAsync wraps SendMessage and SendDirectMessage helping keep bot code more readable.
            /// </summary>
            /// <param name="Message">The message you want to send</param>
            /// <param name="Direct">Whether or not you want the mssage to be a direct message. Default: false</param>
            public async Task RespondAsync(string Message, bool Direct = false) =>
                await Client.SendMessage(Message);
        }
    }
}
