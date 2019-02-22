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
    [Group("admin")]
    class AdminModule
    {
        [Command("leave")]
        [Description("Leave a room")]
        public async Task Leave(Context ctx)
        {
            if (!Program.IsAtagait(ctx.Author))
                return;

            await (ctx.Client.GiveHost(ctx.Author));
            string Ret = await ctx.Client.LeaveRoom();
            if (Ret == "Cannot Leave")
            {
                await ctx.RespondAsync("Cannot leave room yet.");
            }
            else
                ctx.Client.Shutdown();
        }

        [Command("handover"), Aliases("givehost")]
        [Description("Gives a user host.")]
        public async Task Handover(Context ctx)
        {
            if (!Program.IsAtagait(ctx.Author))
                return;
            await (ctx.Client.GiveHost(ctx.Author));
        }
    }

    class TestModule
    {
        [Command("ping")] // define a command
        [Description("Ping")] // define it's description
        [Aliases("pong")] // other strings you can call the command with
        public async Task Ping(Context ctx) // this command takes no arguments, only context.
        {
            await ctx.RespondAsync($"Pong.");
        }

        [Command("lenny")]
        [Description("For when you're too lasy to type them out on your own.")]
        public async Task Lenny(Context ctx)
        {
            await ctx.RespondAsync("( ͡° ͜ʖ ͡°)");
        }

        [Command("shrug")]
        [Description("For when you're too lasy to type them out on your own.")]
        public async Task Shrug(Context ctx)
        {
            await ctx.RespondAsync("¯\\_(ツ)_/¯");
        }

        [Command("roll"), Description("Rolls <N> dice with <X> sides. Format: NdX.")]
        public async Task RollDice(Context ctx, string Dice)
        {
            if (!Regex.IsMatch(Dice, @"\d+d\d+"))
                await ctx.RespondAsync("Your dice format was wrong. The format is NdX. If you want to roll a D20, it would be '1d20'");
            else
            {
                string[] Die = Dice.Split("d");
                int Num = int.Parse(Die[0]);
                int Max = int.Parse(Die[1]);
                int[] results = new int[Num];
                for (int i = 0; i < Num; i++)
                {
                    results[i] = Program.rnd.Next(1, Max);
                }

                await ctx.RespondAsync($"You rolled: {results.Sum().ToString()}");
            }
        }
    }
}
