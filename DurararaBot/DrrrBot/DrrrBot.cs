using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        public bool Running { get; private set; }
        public string CommandPrefix = "#";
        
        private async Task ProcessCommands(object sender, DrrrMessage message)
        {
            if (!message.Text.StartsWith(CommandPrefix))
                return;
            var cmdParams = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            var cmd = cmdParams.Pop(0).ToLower().Substring(CommandPrefix.Length);

            if(Commands.TryGetValue(cmd, out Command command))
            {
                
            }
        }

        public DrrrBot() : base()
        {
            // Add the event handler for command recognition
            OnMessage += ProcessCommands;
        }
    }
}
