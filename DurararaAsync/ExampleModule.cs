using DrrrAsync.Extensions;
using DrrrAsync.Extensions.Attributes;
using DrrrAsync.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExampleBot
{
    class ExampleModule
    {
        [Command("ping")] // define a command
        [Description("Ping")] // define it's description
        [Aliases("pong")] // other strings you can call the command with
        public async Task Ping(Context ctx) // this command takes no arguments, only context.
        {
            await ctx.RespondAsync($"Pong.");
        }

        [Command("echo")]
        [Description("Monkey see, monkey do")]
        public async Task echo(Context ctx, [Remaining] string String) // this command takes Context and a string.
        {
            await ctx.RespondAsync(String);
        }
    }
}
