using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        private readonly List<(Delegate d, string EventName)> EventHandlers = new List<(Delegate d, string EventName)>();
        public readonly Dictionary<string, Command> RegisteredCommands = new Dictionary<string, Command>();

        public void Register(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Add(command.Name, command);
            foreach (var (d, EventName) in module.EventHandlers)
            {
                EventHandlers.Add((d, EventName));
                GetType().GetEvent(EventName).AddEventHandler(this, d);
            }
            Logger.Info($"Registered Module: {module.Name}");
        }

        public void Unregister(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Remove(command.Name);
            foreach (var t in module.EventHandlers)
            {
                EventHandlers.Remove(t);
                GetType().GetEvent(t.EventName).RemoveEventHandler(this, t.d);
            }
            Logger.Info($"Unregistered Module: {module.Name}");
        }
    }
}
