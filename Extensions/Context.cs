using System;
using System.Collections.Generic;
using System.Text;

using DrrrAsync;
using DrrrAsync.Objects;

namespace DrrrAsync
{
    namespace Extensions
    {
        public class Context
        {
            public DrrrMessage Message { get; private set; }
            public DrrrUser Author { get; private set; }
            public DrrrRoom Room { get; private set; }

            public DrrrClient Client { get; private set; }

            public Context(DrrrClient Client, DrrrMessage aMessage, DrrrUser aAuthor, DrrrRoom aRoom)
            {
                Message = aMessage;
                Author = aAuthor;
                Room = aRoom;
            }

            public async void RespondAsync(string Message) =>
                await Client.SendMessage(Message);
        }
    }
}
