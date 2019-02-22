using System;
using System.Reflection;
using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{

    /// <summary>
    /// The command class is used for handling bot commands.
    /// It contains the command's name, description, as well as a
    /// reference to it's container module, and handler.
    /// </summary>
    public class Command
    {
        public readonly string Name;
        public readonly string Description;
        private readonly Delegate Handler;
        public Module Module;
        public readonly CommandAuthority Authority;
        
        /// <param name="module">The module the command is in</param>
        /// <param name="Cmd">The method the command is linked to</param>
        /// <param name="description">The command's description</param>
        public Command(Module module, Delegate handler, string name, string description, CommandAuthority authority)
        {
            Module = module;
            Handler = handler;
            Name = name;
            Description = description;
            Authority = authority;
        }

        /// <summary>Executes the command.</summary>
        /// <param name="e">The event arguments, passed as a tuple.</param>
        public async Task Execute(object[] args) =>
            await (Task) Handler.DynamicInvoke(args);
    }
}
