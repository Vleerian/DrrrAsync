using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{
    public struct CommandHandlerArgs
    {
        public readonly string[] Args;
        public readonly DrrrMessage Message;
        public readonly DrrrBot Bot;

        public CommandHandlerArgs(string[] args, DrrrUser author, DrrrRoom room, DrrrMessage message, DrrrBot bot)
        {
            Args = args;
            Message = message;
            Bot = bot;
        }
    }
}
