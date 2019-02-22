using System;
using System.Threading.Tasks;

using DrrrAsync.Bot;

namespace ExampleBot
{
    class NewTestModule : Module
    {
        public static NewTestModule Instance = new NewTestModule();

        [Command("time")]
        async public Task CommandTime(CommandHandlerArgs e)
        {
            await e.Bot.SendMessage(DateTime.Now.ToShortTimeString());
        }

        public NewTestModule() : base("TestModule") { }
    }
}
