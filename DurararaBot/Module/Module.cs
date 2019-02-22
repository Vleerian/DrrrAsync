using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace DrrrAsync.Bot
{
    abstract public class Module
    {
        private readonly IEnumerable<MethodInfo> Methods;
        public readonly List<Command> Commands = new List<Command>();
        public readonly List<(string EventName, MethodInfo Method, Module Module)> EventHandlers = new List<(string, MethodInfo, Module)>();
        
        public Module()
        {
            // Register Commands and Event Handlers to the Module
            foreach(var MethodInfo in GetType().GetRuntimeMethods())
            {
                if (MethodInfo.GetCustomAttribute<CommandAttribute>() is CommandAttribute commandAttribute)
                    Commands.Add(
                        new Command(
                            this, 
                            (CommandHandler) MethodInfo.CreateDelegate(typeof(CommandHandler), this), 
                            commandAttribute));
                if (MethodInfo.GetCustomAttribute<EventHandlerAttribute>() is EventHandlerAttribute eventAttribute)
                    EventHandlers.Add((eventAttribute.EventName, MethodInfo, this));
            }
        }
    }
}
