using DrrrAsync.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DrrrAsync.AsyncEvents;

namespace DrrrAsync
{
    namespace Extensions
    {
        public class Bot
        {
            public class Command
            {
                public dynamic Module;
                MethodInfo Method;
                public string Name { get; private set; }
                public string Description { get; private set; }
                
                public Command(dynamic aModule, MethodInfo Cmd, string aName, string aDescription = "")
                {
                    Module = aModule;
                    Method = Cmd;
                    Name = aName;
                    Description = aDescription;
                }

                public async Task Call(Context ctx) =>
                    await Method.Invoke(Module, new object[] { ctx });
            }

            private Dictionary<string, Command> Commands;

            public DrrrClient Client { get; private set; }

            public Bot(string aName, string aIcon)
            {
                Client = new DrrrClient(aName, aIcon);

                Client.On_Message.Register(LookForCommands);
            }

            public void RegisterCommands<T>(T CommandModule) where T : class
            {
                Type ClassType = CommandModule.GetType();

                foreach (MethodInfo Method in ClassType.GetMethods())
                {
                    Attributes.Command Cmd = Method.GetCustomAttribute<Attributes.Command>();
                    if(Cmd != null)
                    {
                        Attributes.Description Desc = Method.GetCustomAttribute<Attributes.Description>();
                        Command cmd = new Command(CommandModule, Method, Cmd.CommandName, Desc!=null?Desc.CommandDescription:"");
                    }
                }
            }

            private async Task LookForCommands(DrrrMessage e)
            {
                string Cmnd = e.Mesg.Split(" ", 1, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
                if (Commands.ContainsKey(Cmnd))
                {
                    Context ctx = new Context(Client, e, (e.From != null) ? e.Usr : e.From, e.PostedIn);
                    Commands[Cmnd].Call(ctx).Start();
                }

                await Task.CompletedTask;
            }
        }
    }
}
