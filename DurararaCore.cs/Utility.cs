using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DrrrAsync
{
    public class Utility
    {
        public static void Col(char col)
        {
            new Dictionary<char, Action>()
            {
                { 'r', () => { Console.ForegroundColor = ConsoleColor.Red;}},
                { 'g', () => { Console.ForegroundColor = ConsoleColor.Green;}},
                { 'b', () => { Console.ForegroundColor = ConsoleColor.Blue;}},
                { 'c', () => { Console.ForegroundColor = ConsoleColor.Cyan;}},
                { 'm', () => { Console.ForegroundColor = ConsoleColor.Magenta;}},
                { 'y', () => { Console.ForegroundColor = ConsoleColor.Yellow;}},
                { 'k', () => { Console.ForegroundColor = ConsoleColor.Black;}},
                { 'w', () => { Console.ForegroundColor = ConsoleColor.White;}},
                { 'R', () => { Console.ForegroundColor = ConsoleColor.DarkRed;}},
                { 'G', () => { Console.ForegroundColor = ConsoleColor.DarkGreen;}},
                { 'B', () => { Console.ForegroundColor = ConsoleColor.DarkBlue;}},
                { 'C', () => { Console.ForegroundColor = ConsoleColor.DarkCyan;}},
                { 'M', () => { Console.ForegroundColor = ConsoleColor.DarkMagenta;}},
                { 'Y', () => { Console.ForegroundColor = ConsoleColor.DarkYellow;}},
                { 'l', () => { Console.ForegroundColor = ConsoleColor.Gray;}},
                { 'L', () => { Console.ForegroundColor = ConsoleColor.DarkGray;}},
            }[col]();
        }

        public static string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, ' ').PadRight(length, ' ');
        }

        public static void Status_box(char code, string text)
        {
            Col('w'); Console.Write("[");
            Col(code); Console.Write(PadBoth(text, 7));
            Col('w'); Console.Write("] ");
        }


        public enum Log_Level
        {
            Debug,
            Basic,
            Status,
            Essential
        }

        public static Log_Level Global_Loglevel = Log_Level.Debug;

        public static void Log(char status_colour, string status, string text, Log_Level LogLevel)
        {
            if (LogLevel >= Global_Loglevel)
            {
                Status_box(status_colour, status);
                Console.Write(text + "\n");
            }
        }
    }
}
