using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Console = Colorful.Console;

namespace DrrrBot.Helpers
{
    class Utilities
    {
        public static void Log(Color col, string status, string text) =>
            Console.WriteLineFormatted($"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}] <{{0}}> {text}", col, Color.White, status.Center(9));

        public static string SnipText(ref string Text, int Position)
        {
            string ReturnText = Text.Substring(0, Position);
            Text = Text.Substring(Position);
            return ReturnText;
        }
    }
}
