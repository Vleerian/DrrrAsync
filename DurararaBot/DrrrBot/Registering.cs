using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

using DrrrAsync.Bot;

namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        private readonly List<(Delegate d, string EventName)> RegisteredEventHandlerDelegates = new List<(Delegate d, string EventName)>();
        public readonly List<Command> RegisteredCommands = new List<Command>();

        public async Task Register(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Add(command);
            foreach (var (EventName, Method, Module) in module.EventHandlers)
            {
                var d = Method.CreateDelegate(Module);
                RegisteredEventHandlerDelegates.Add((d, EventName));
                GetType().GetEvent(EventName).AddEventHandler(this, d);
            }
        }

        public async Task Unregister(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Remove(command);
            foreach (var t in RegisteredEventHandlerDelegates)
            {
                RegisteredEventHandlerDelegates.Remove(t);
                GetType().GetEvent(t.EventName).RemoveEventHandler(this, t.d);
            }
        }
    }
}
