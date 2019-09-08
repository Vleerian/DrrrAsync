using System;

using DrrrBot.Objects;
using DrrrBot.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using DrrrBot.Permission;
using DrrrBot.Helpers;
using System.Drawing;
using System.Linq;

namespace DrrrBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            DrrrBotConfig Config;
            var json = "";
            using (var fs = File.OpenRead("./config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            Config = JsonConvert.DeserializeObject<DrrrBotConfig>(json);
            var drrrBot = Bot.Create(Config);

            await drrrBot.Register<ExampleModule>();

            await drrrBot.Run();
        }
    }
}
