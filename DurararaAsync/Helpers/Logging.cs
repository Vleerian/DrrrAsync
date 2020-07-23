using System;
using System.IO;
using System.Reflection;

using log4net;
using log4net.Config;

namespace DrrrAsyncBot.Helpers
{
    public class ColorLogLevel
    {
        public static readonly ColorLogLevel fatal = new ColorLogLevel(log4net.Core.Level.Fatal, ConsoleColor.Red);
        public static readonly ColorLogLevel error = new ColorLogLevel(log4net.Core.Level.Error, ConsoleColor.Yellow);
        public static readonly ColorLogLevel warn = new ColorLogLevel(log4net.Core.Level.Warn, ConsoleColor.Yellow);
        public static readonly ColorLogLevel done = new ColorLogLevel(logExtensions.doneLevel, ConsoleColor.Green);
        public static readonly ColorLogLevel info = new ColorLogLevel(log4net.Core.Level.Info, ConsoleColor.Cyan);
        public static readonly ColorLogLevel debug = new ColorLogLevel(log4net.Core.Level.Debug, ConsoleColor.Blue);

        public readonly log4net.Core.Level level;
        public readonly ConsoleColor color;

        public ColorLogLevel(log4net.Core.Level Level, ConsoleColor Color)
        {
            level = Level;
            color = Color;
        }

        public static implicit operator ColorLogLevel(log4net.Core.Level level)
        {
            switch(level.Name.ToLower())
            {
                case "fatal": return fatal;
                case "error": return error;
                case "warn": return warn;
                case "done": return done;
                case "info": return info;
                case "debug": return debug;
                default: return info;
            }
        }
    }

    public class Logger
    {
        private static Type CallerStack = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly ILog logger = LogManager.GetLogger(CallerStack);

        public static void Configure(){
            // Load configuration
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("./log4net.config"));

            Done("Logger has been set up.");
        }

        private static void log(ColorLogLevel level, string message, Exception exception = null)
        {
            string ExceptionLine = (exception != null) ? $" - {exception.ToString()}" : "";
            Console.Write($"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}] <");
            Console.ForegroundColor = level.color;
            Console.Write(level.level.Name.ToUpper().Center(13));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"> {message}{ExceptionLine}\n");
            if (exception != null && level.level.Value <= 30000)
                Console.WriteLine(exception.StackTrace);

            logger.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, level.level, message, exception);
        }

        public static void Fatal(string message) =>
            log(ColorLogLevel.fatal, message);

        public static void Fatal(string message, Exception exception) =>
            log(ColorLogLevel.fatal, message, exception);
        
        public static void Error(string message) =>
            log(ColorLogLevel.error, message);

        public static void Error(string message, Exception exception) =>
            log(ColorLogLevel.error, message, exception);

        public static void Warn(string message) =>
            log(ColorLogLevel.warn, message);

        public static void Warn(string message, Exception exception) =>
            log(ColorLogLevel.warn, message, exception);

        public static void Done(string message) =>
            log(ColorLogLevel.done, message);

        public static void Done(string message, Exception exception) =>
            log(ColorLogLevel.done, message, exception);

        public static void Info(string message) =>
            log(ColorLogLevel.info, message);

        public static void Info(string message, Exception exception) =>
            log(ColorLogLevel.info, message, exception);

        public static void Debug(string message) =>
            log(ColorLogLevel.debug, message);

        public static void Debug(string message, Exception exception) =>
            log(ColorLogLevel.debug, message, exception);
    }
}
