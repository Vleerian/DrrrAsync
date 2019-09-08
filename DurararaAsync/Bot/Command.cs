using System.Reflection;
using System.Threading.Tasks;

using DrrrBot.Permission;

namespace DrrrBot.Core
{
    public class Command
    {
        public dynamic Module;
        public MethodInfo Method { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PermLevel Permission { get; private set; }

        /// <summary>
        /// The Command class constructor. It instantiates all member variables.
        /// </summary>
        /// <param name="aModule">The module the command is in</param>
        /// <param name="Cmd">The method the command is linked to</param>
        /// <param name="aName">The command's name</param>
        /// <param name="aDescription">The command's description</param>
        public Command(dynamic aModule, MethodInfo Cmd, string aName, string aDescription = "", PermLevel aPerm = 0)
        {
            Module = aModule;
            Method = Cmd;
            Name = aName;
            Description = aDescription;
            Permission = aPerm;
        }

        /// <summary>
        /// Invokes the command
        /// </summary>
        /// <param name="args">An Array of Parameters for the invokation.</param>
        public Task Call(object[] args) =>
            Method.Invoke(Module, args);
    }
}
