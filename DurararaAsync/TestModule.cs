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

        [Command("a")]
        public async Task ree(Context ctx, string bones)
        {
            await ctx.RespondAsync(bones);
        }

        [Command("b")]
        public async Task faa(Context ctx, [Remaining] string bones)
        {
            await ctx.RespondAsync(bones);
        }

        [Command("c")]
        public async Task laa(Context ctx, int bones)
        {
            await ctx.RespondAsync((bones + 1).ToString());
        }
    }
}
