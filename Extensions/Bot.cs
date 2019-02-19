using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    namespace Extensions
    {
        public class Bot
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

            // A dictionary of {"CommandName":CommandObject} used to invoke executed commands.
            private Dictionary<string, Command> Commands;

            // The client object the bot uses
            public DrrrClient Client { get; private set; }

            /// <summary>
            /// The bot constructor instantiates the client, as well as registering it's command processor as an event.
            /// </summary>
            /// <param name="aName">The name the bot will use on Drrr.com</param>
            /// <param name="aIcon">The icon the bot will use on Drrr.com</param>
            public Bot(string aName, string aIcon)
            {
                Client = new DrrrClient(aName, aIcon);
                Client.On_Message.Register(LookForCommands);
            }

            /// <summary>
            /// The register command goes through all the methods in command module, and those with the Command attribute
            /// are added to the CommandDictionary.
            /// </summary>
            /// <typeparam name="T">The CommandModule's type</typeparam>
            /// <param name="CommandModule">The command module.</param>
            public void RegisterCommands<T>(T CommandModule) where T : class
            {
                // Get the command module's type, and iterate through it's methods
                Type ClassType = CommandModule.GetType();
                foreach (MethodInfo Method in ClassType.GetMethods())
                {
                    // If it has the Command attribute, add it to the CommandDictionary
                    Attributes.Command CommandAttribute = Method.GetCustomAttribute<Attributes.Command>();
                    if(CommandAttribute != null)
                    {
                        Attributes.Description Desc = Method.GetCustomAttribute<Attributes.Description>();
                        Command CommandObject = new Command(CommandModule, Method, CommandAttribute.CommandName, Desc!=null?Desc.CommandDescription:"");

                        // TODO: Check for Aliases attribute, and iterate through them.
                        Commands.Add(CommandAttribute.CommandName, CommandObject);
                    }
                }
            }

            /// <summary>
            /// The bot's command processor runs whenever a DrrrMessage event is thrown, and looks for commands.
            /// </summary>
            /// <param name="e">The DrrrMessage event.</param>
            private async Task LookForCommands(DrrrMessage e)
            {
                // Check if the message starts with a command.
                // TODO: check for a command signal (I.E mesg.startswith(CommandSignal) where CommandSignal = '#')
                string Cmnd = e.Mesg.Split(" ", 1, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
                if (Commands.ContainsKey(Cmnd))
                {
                    // If a command or alias is in the CommandDictionary, execute it.
                    Context ctx = new Context(Client, e, (e.From != null) ? e.Usr : e.From, e.PostedIn);
                    Commands[Cmnd].Call(ctx).Start();
                }

                await Task.CompletedTask;
            }
        }
    }
}
