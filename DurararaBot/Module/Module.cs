using System;
using System.Reflection;
using System.Collections.Generic;

using DrrrAsync.Logging;

namespace DrrrAsync.Bot
{
    abstract public class Module
    {
        public string Name { get; private set; }
        protected readonly Logger Logger = new Logger();

        public readonly List<Command> Commands = new List<Command>();
        public readonly List<(Delegate d, string EventName)> EventHandlers = new List<(Delegate d, string EventName)>();
        
        public Module(string name)
        {
            Name = name;
            Logger.Name = name;

            // Register Commands and Event Handlers to the Module
            foreach(var method in GetType().GetRuntimeMethods())
            {
                if (method.Has(out CommandAttribute cmdAttr))
                {
                    Commands.Add(new Command(this, method.CreateDelegate(this), cmdAttr.Name, cmdAttr.Description, cmdAttr.Authority, cmdAttr.StringSeperator, cmdAttr.ParseStrings));
                    foreach (string alias in cmdAttr.Aliases)
                        Commands.Add(new Command(this, method.CreateDelegate(this), alias, cmdAttr.Description, cmdAttr.Authority, cmdAttr.StringSeperator, cmdAttr.ParseStrings));
                }
                if (method.Has(out EventHandlerAttribute eventAttribute))
                    EventHandlers.Add((method.CreateDelegate(this), eventAttribute.EventName));
            }
        }
    }
}
