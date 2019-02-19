using System;

namespace DrrrAsync
{
    public class Utility
    {
        /// <summary>
        /// Changes the console color. Wraps Console.ForegroundColor
        /// </summary>
        /// <param name="col">The first letter of a color. Lowercase for bright, uppercase for dark. k for black, l for gray.</param>
        public static void Col(char col)
        {
            switch (col)
            {
                case 'r':
                    Console.ForegroundColor = ConsoleColor.Red; break;
                case 'g':
                    Console.ForegroundColor = ConsoleColor.Green; break;
                case 'b':
                    Console.ForegroundColor = ConsoleColor.Blue; break;
                case 'c':
                    Console.ForegroundColor = ConsoleColor.Cyan; break;
                case 'm':
                    Console.ForegroundColor = ConsoleColor.Magenta; break;
                case 'y':
                    Console.ForegroundColor = ConsoleColor.Yellow; break;
                case 'k':
                    Console.ForegroundColor = ConsoleColor.Black; break;
                case 'w':
                    Console.ForegroundColor = ConsoleColor.White; break;
                case 'l':
                    Console.ForegroundColor = ConsoleColor.Gray; break;
                case 'R':
                    Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case 'G':
                    Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case 'B':
                    Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                case 'C':
                    Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                case 'M':
                    Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                case 'Y':
                    Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case 'L':
                    Console.ForegroundColor = ConsoleColor.DarkGray; break;
                default:
                    Console.ForegroundColor = ConsoleColor.White; break;

            }
        }

        /// <summary>
        /// Pads both sides of a string to center it.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="length">How long you want the end string to be.</param>
        /// <returns>The padded string.</returns>
        public static string PadBoth(string source, int length)
        {
            if (source.Length > length)
                throw new ArgumentException("Source string is longer than provided length.");
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, ' ').PadRight(length, ' ');
        }

        /// <summary>
        /// Generates a colourful status box.
        /// </summary>
        /// <param name="code">The color code (see Utility.Col)</param>
        /// <param name="text">The text you want inside the status box.</param>
        public static void Status_box(char code, string text)
        {
            Col('w'); Console.Write("[");
            Col(code); Console.Write(PadBoth(text, 7));
            Col('w'); Console.Write("] ");
        }

        // Controls what messages are sent to the console.
        public enum Log_Level
        {
            Verbose,    // Send verbose debug-level messages
            Debug,      // Send basic debug-level messages
            Status,     // Send status messages only
            Essential   // Send only essential information
        }

        // The log-level used program-wide.
        public static Log_Level Global_Loglevel = Log_Level.Verbose;

        /// <summary>
        /// Print a log messages to the console. Dependent on Global_Loglevel.
        /// </summary>
        /// <param name="status_colour">The color of the message in the status box. (See Utility.Col)</param>
        /// <param name="status">The message you want in the status box.</param>
        /// <param name="text">The message you want to send</param>
        /// <param name="LogLevel">What level of message is this.</param>
        public static void Log(char status_colour, string status, string text, Log_Level LogLevel)
        {
            // If the log level passed into the function is at or higher than the global, print the message.
            if (LogLevel >= Global_Loglevel)
            {
                Status_box(status_colour, status);
                Console.Write(text + "\n");
            }
        }
    }
}
