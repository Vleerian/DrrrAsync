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
                case 'r': Console.ForegroundColor = ConsoleColor.Red; break;
                case 'g': Console.ForegroundColor = ConsoleColor.Green; break;
                case 'b': Console.ForegroundColor = ConsoleColor.Blue; break;
                case 'c': Console.ForegroundColor = ConsoleColor.Cyan; break;
                case 'm': Console.ForegroundColor = ConsoleColor.Magenta; break;
                case 'y': Console.ForegroundColor = ConsoleColor.Yellow; break;
                case 'k': Console.ForegroundColor = ConsoleColor.Black; break;
                case 'w': Console.ForegroundColor = ConsoleColor.White; break;
                case 'l': Console.ForegroundColor = ConsoleColor.Gray; break;
                case 'R': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case 'G': Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case 'B': Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                case 'C': Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                case 'M': Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                case 'Y': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case 'L': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                default: Console.ForegroundColor = ConsoleColor.White; break;
            }
        }
    }

    /// <summary>
    /// Holding class for Utility Extensions
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// Pads both sides of a string to center it.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="length">How long you want the end string to be.</param>
        /// <returns>The padded string.</returns>
        public static string PadBoth(this string source, int length)
        {
            if (source.Length > length)
                throw new ArgumentException("Source string is longer than provided length.");
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, ' ').PadRight(length, ' ');
        }
    }
}
