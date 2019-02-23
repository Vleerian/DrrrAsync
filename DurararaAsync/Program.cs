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
            DrrrUser Author = Message.From ?? Message.Usr;

            string Mes;

            switch (Message.Type)
            {
                case "message": Mes = $"{Author.Name}: {Message.Text}"; break;
                case "me": Mes = Message.Text.Replace("{1}", Author.Name).Replace("{2", Message.Content); break;
                case "roll": Mes = Author.Name + " Rolled " + Message.To.Name; break;
                case "music": Mes = $"{Author.Name} shared music."; break;
                case "kick":
                case "ban":
                    Mes = $"{Message.Type.ID.ToUpper()}ED - {Author.Name}"; break;
                case "join":
                case "leave":
                    Mes = $"{Message.Type.ID.ToUpper()} - {Author.Name}"; break;
                case "room-profile":
                    Mes = "Room Updated."; break;
                default:
                    Mes = $"[{Message.Type}] {Message.Text}"; break;
            }

            Console.WriteLine($"{Timestamp} -- {Mes}");

            await File.AppendAllTextAsync($"./logs/{Message.PostedIn.Name}.log", $"{Timestamp} -- {Mes}");
        }
    }
}
