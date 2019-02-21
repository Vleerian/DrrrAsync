using System.Reflection;
using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{
    public delegate Task CommandHandler(CommandHandlerArgs e);

    /// <summary>
    /// The command class is used for handling bot commands.
    /// It contains the command's name, description, as well as a
    /// reference to it's container module, and handler.
    /// </summary>
    public class Command
    {
        public readonly string Name, Description;
        public readonly string[] Aliases;
        private readonly CommandHandler Handler;
        public Module Module;
        public readonly CommandAuthority Authority;
        
        /// <param name="module">The module the command is in</param>
        /// <param name="Cmd">The method the command is linked to</param>
        /// <param name="name">The command's name</param>
        /// <param name="description">The command's description</param>
        public Command(Module module, CommandHandler handler, CommandAttribute attribute)
        {
            Module = module;
            Handler = handler;
            Name = attribute.Name;
            Description = attribute.Description;
            Aliases = attribute.Aliases;
            Authority = attribute.Authority;
        }

        /// <summary>Executes the command.</summary>
        /// <param name="e">The event arguments, passed as a tuple.</param>
        public async Task Execute(string[] args, DrrrUser author, DrrrRoom room, DrrrMessage message, DrrrClient client) =>
            await Handler(new CommandHandlerArgs(args, author, room, message, client));
    }
}
