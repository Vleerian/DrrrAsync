using System.Reflection;
using System.Threading.Tasks;

namespace DrrrAsync
{
    namespace Extensions
    {
        public partial class Bot
        {
            /// <summary>
            /// The command class is used for handling bot commands.
            /// It contains the command's name, description, as well as a
            /// reference to it's container module, and method.
            /// </summary>
            public class Command
            {
                public dynamic Module;
                MethodInfo Method;
                public string Name { get; private set; }
                public string Description { get; private set; }
                
                /// <summary>
                /// The Command class constructor. It instantiates all member variables.
                /// </summary>
                /// <param name="aModule">The module the command is in</param>
                /// <param name="Cmd">The method the command is linked to</param>
                /// <param name="aName">The command's name</param>
                /// <param name="aDescription">The command's description</param>
                public Command(dynamic aModule, MethodInfo Cmd, string aName, string aDescription = "")
                {
                    Module = aModule;
                    Method = Cmd;
                    Name = aName;
                    Description = aDescription;
                }

                /// <summary>
                /// Invokes the command
                /// </summary>
                /// <param name="ctx">The context object the command will use</param>
                /// TODO: Parse ctx.message and pass the command a list of arguments.
                public async Task Call(Context ctx) =>
                    await Method.Invoke(Module, new object[] { ctx });
            }
        }
    }
}
