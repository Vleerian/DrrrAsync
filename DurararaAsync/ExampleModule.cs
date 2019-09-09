using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using DrrrAsyncBot.Core;
using DrrrAsyncBot.Helpers;

using System;

namespace DrrrAsyncBot
{
    class ExampleModule
    {
        [Command("ping")] // define a command
        [Description("Ping")] // define it's description
        [Aliases("pong")] // other strings you can call the command with
        public async Task Ping(Context ctx, params string[] User) // this command takes no arguments, only context.
        {
            await ctx.RespondAsync($"Pong!");
        }
    }
}
