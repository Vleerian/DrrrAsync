using System;

namespace DrrrAsync.Logging
{
    /// <summary>
    /// Logging class that should be unique to each "thing" that logs an action.
    /// </summary>
    public class Logger
    {
        public string Name = "Log";
        public bool LogTime = true;
        public LogLevel LogLevel = LogLevel.DEBUG;
        private static readonly object Lock = new object();

        /// <summary>
        /// Logs a message to the Console.
        /// </summary>
        /// <typeparam name="T">Not to be specified => Converts automatically to a string.</typeparam>
        /// <param name="message">The string or object to be logged.</param>
        /// <param name="level">The level to log at.</param>
        public void Log<T>(T message, LogLevel level)
        {
            if (level <= LogLevel)
                lock (Lock)
                {
                    Console.ForegroundColor = level.Color;
                    if (LogTime) Console.Write($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] ");
                    Console.Write($"[{Name} {level.Name}]: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message.ToString());
                }
            else return;
        }

        public void Fatal<T>(T message) => Log(message, LogLevel.FATAL);
        public void Error<T>(T message) => Log(message, LogLevel.ERROR);
        public void Warn<T>(T message) => Log(message, LogLevel.WARN);
        public void Info<T>(T message) => Log(message, LogLevel.INFO);
        public void Debug<T>(T message) => Log(message, LogLevel.DEBUG);
    }
}
