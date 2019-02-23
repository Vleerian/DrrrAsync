using System;
using System.Threading.Tasks;

using DrrrAsync;
using DrrrAsync.Objects;
using DrrrAsync.Extensions;
using System.IO;

namespace ExampleBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("What to call the bot: ");
            Bot DrrrBot = new Bot { Name = Console.ReadLine(), Icon = DrrrIcon.Kuromu2x, CommandPrefix = "#" };
            DrrrBot.Register<ExampleModule>();

            await DrrrBot.Login();

            Console.Write("What room to connect to: ");
            await DrrrBot.Connect(Console.ReadLine());

            while (DrrrBot.Running) ;
            Console.WriteLine("Bot has finished running.");
            Console.ReadKey();
        }
    }
}
