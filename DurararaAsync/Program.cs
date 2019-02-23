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
        //We define random statically because it helps make random numbers slightly more random
        public static Random rnd = new Random();

        public static bool IsAtagait(DrrrUser user) => user.Tripcode == "Pride/2aQQ";

        static async Task Main(string[] args)
        {
            var mod = NewTestModule.Instance;
            Bot DrrrBot = new Bot { Name = "Welne Oren", Icon = DrrrIcon.Kuromu2x, CommandPrefix = "#" };
            DrrrBot.Register<TestModule>();
            DrrrBot.Register<AdminModule>();

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            DrrrBot.OnMessage += PrintMessages;

            await DrrrBot.Login();
            await DrrrBot.Connect("WIZARD TOWERs");

            while (DrrrBot.Running)
                await Task.Delay(1000);
        }

        private async static Task PrintMessages(object Sender, DrrrMessage Message)
        {
            string Timestamp = $"<{Message.Timestamp.ToShortTimeString()}> ";

            string Mes;

            switch (Message.Type)
            {
                case "message": Mes = $"{Message.Author.Name}: {Message.Text}"; break;
                case "me": Mes = Message.Text.Replace("{1}", Message.Author.Name).Replace("{2", Message.Content); break;
                case "roll": Mes = Message.Author.Name + " Rolled " + Message.Target.Name; break;
                case "music": Mes = $"{Message.Author.Name} shared music."; break;
                case "kick":
                case "ban":
                    Mes = $"{Message.Type.ID.ToUpper()}ED - {Message.Author.Name}"; break;
                case "join":
                case "leave":
                    Mes = $"{Message.Type.ID.ToUpper()} - {Message.Author.Name}"; break;
                case "room-profile":
                    Mes = "Room Updated."; break;
                default:
                    Mes = $"[{Message.Type}] {Message.Text}"; break;
            }

            Console.WriteLine($"{Timestamp} -- {Mes}");

            await File.AppendAllTextAsync($"./logs/{Message.Room.Name}.log", $"{Timestamp} -- {Mes}");
        }
    }
}
