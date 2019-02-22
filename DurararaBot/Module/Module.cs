using System;
using System.Reflection;
using System.Collections.Generic;

namespace DrrrAsync.Bot
{
    abstract public class Module
    {
        public readonly List<Command> Commands = new List<Command>();
        public readonly List<(Delegate d, string EventName)> EventHandlers = new List<(Delegate d, string EventName)>();
        
        public Module()
        {
            // Register Commands and Event Handlers to the Module
            foreach(var method in GetType().GetRuntimeMethods())
            {
                if (method.GetCustomAttribute<CommandAttribute>() is var cmdAttr)
                {
                    Commands.Add(new Command(this, method.CreateDelegate(this), cmdAttr.Name, cmdAttr.Description, cmdAttr.Authority));
                    foreach (string alias in cmdAttr.Aliases)
                        Commands.Add(new Command(this, method.CreateDelegate(this), alias, cmdAttr.Description, cmdAttr.Authority));
                }
                if (method.GetCustomAttribute<EventHandlerAttribute>() is var eventAttribute)
                    EventHandlers.Add((method.CreateDelegate(this), eventAttribute.EventName));
            }
        }
    }
}
