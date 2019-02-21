using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{
    public struct CommandHandlerArgs
    {
        public readonly string[] Args;
        public readonly DrrrUser Author;
        public readonly DrrrRoom Room;
        public readonly DrrrMessage Message;
        public readonly DrrrClient Client;

        public CommandHandlerArgs(string[] args, DrrrUser author, DrrrRoom room, DrrrMessage message, DrrrClient client)
        {
            Args = args;
            Author = author;
            Room = room;
            Message = message;
            Client = client;
        }
    }
}
