using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        public readonly Dictionary<string, Command> RegisteredCommands = new Dictionary<string, Command>();

        public void Register(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Add(command.Name, command);
            foreach (var (d, EventName) in module.EventHandlers)
                GetType().GetEvent(EventName).AddEventHandler(this, d);
            Logger.Info($"Registered Module: {module.Name}");
        }

        public void Unregister(Module module)
        {
            foreach (var command in module.Commands) RegisteredCommands.Remove(command.Name);
            foreach (var t in module.EventHandlers)
                GetType().GetEvent(t.EventName).RemoveEventHandler(this, t.d);
            Logger.Info($"Unregistered Module: {module.Name}");
        }
    }
}
