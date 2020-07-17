using System;
using System.Collections.Generic;
using System.Text;
using DrrrAsyncBot.Helpers;

namespace DrrrAsyncBot.Helpers
{
    public enum LogEventType
    {
        None,
        Fatal,
        Error,
        Warning,
        Done,
        Information,
        Debug,
        Verbose
    }

    public static class Logger
    {
        public static LogEventType logLevel;

        public static void Log(LogEventType logEventType, string Message, Exception exception = null)
        {
            if (logEventType > logLevel)
                return;

            ConsoleColor statusColor;
            switch (logEventType)
            {
                case LogEventType.Fatal:
                case LogEventType.Error:
                    statusColor = ConsoleColor.Red;
                    break;
                case LogEventType.Warning:
                    statusColor = ConsoleColor.Yellow;
                    break;
                case LogEventType.Done:
                    statusColor = ConsoleColor.Green;
                    break;
                case LogEventType.Information:
                    statusColor = ConsoleColor.Cyan;
                    break;
                case LogEventType.Debug:
                    statusColor = ConsoleColor.Blue;
                    break;
                default:
                    statusColor = ConsoleColor.White;
                    break;
            }


            string ExceptionLine = (exception != null) ? $" - {exception.ToString()}" : "";
            Console.Write($"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}] <");
            Console.ForegroundColor = statusColor;
            Console.Write(logEventType.ToString().ToUpper().Center(13));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"> {Message}{ExceptionLine}\n");
            if (exception != null && logLevel == LogEventType.Debug)
                Console.WriteLine(exception.StackTrace);
        }
    }
}
