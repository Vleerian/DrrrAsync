using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Console = Colorful.Console;

namespace DrrrBot.Helpers
{
    /// <summary>
    /// A container for frequently used functions.
    /// </summary>
    class Utilities
    {
        /// <summary>
        /// Outputs pleasing-looking timestamped log messages, with a status message
        /// </summary>
        /// <param name="col">System.Drawing.Color of the status</param>
        /// <param name="status">The status type of the message</param>
        /// <param name="text">The message itself</param>
        public static void Log(Color col, string status, string text) =>
            Console.WriteLineFormatted($"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}] <{{0}}> {text}", col, Color.White, status.Center(9));
    }
}
