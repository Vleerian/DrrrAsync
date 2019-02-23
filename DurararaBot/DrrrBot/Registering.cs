using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        public readonly Dictionary<string, Command> Commands= new Dictionary<string, Command>();

        public void Register(Module module)
        {
            foreach (var command in module.Commands) Commands.Add(command.Name, command);
            foreach (var (d, EventName) in module.EventHandlers)
                GetType().GetEvent(EventName).AddEventHandler(this, d);
            Logger.Info($"Registered Module: {module.Name}");
        }

        public void Unregister(Module module)
        {
            foreach (var command in module.Commands) Commands.Remove(command.Name);
            foreach (var (d, EventName) in module.EventHandlers)
                GetType().GetEvent(EventName).RemoveEventHandler(this, d);
            Logger.Info($"Unregistered Module: {module.Name}");
        }
    }
}
