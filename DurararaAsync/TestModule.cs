using DrrrAsync.Extensions;
using DrrrAsync.Extensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurararaAsync
{
    class TestModule
    {
        [Command("ping")]
        public async Task Ping(Context ctx)
        {
            await ctx.RespondAsync("Pong!");
        }
    }
}
