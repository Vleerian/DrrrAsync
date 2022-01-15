using System;
using System.IO;

namespace DrrrAsync.Logging
{
    /// <summary>
	/// A basic logging class that implements ILogger
	/// </summary>
    /// <remarks>
    /// Inheriting from BasicLogger instead of ILogger is reccomended,
    /// even if only to avoid re-implementing the typesafe log methods
    /// </remarks>
	/// <author>Atagait Denral</author>
    public class BasicLogger : ILogger
    {
        public LogLevel threshhold;
        public readonly string LoggerName;
        
        private readonly string logFile;

        public BasicLogger(string Name, LogLevel Threshhold, string LogFile = null)
        {
            LoggerName = Name;
            threshhold = Threshhold;
            logFile = LogFile;
        }

        public void Log(LogLevel logLevel, string Message, Exception exception = null)
        {
            
            if(logLevel < threshhold)
                return;
            string Timestamp = DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss");

            writeLogToConsole(Timestamp, logLevel, Message, exception);
            writeLogToFile(Timestamp, logLevel, Message, exception);
        }
        
        // Utility function to center log names
        public static string CenteredLogname(LogLevel level)
        {
            string LogName = level.ShortName;
            int spaces = 9 - LogName.Length;
            int padLeft = spaces / 2 + LogName.Length;
            return LogName.PadLeft(padLeft, ' ').PadRight(9, ' ');
        }

        // Utility function for ease of writing out the coloured log level name
        protected void writeLogToConsole(string Timestamp, LogLevel level, string Message, Exception exception)
        {
            Console.Write($"[{Timestamp}] {LoggerName} <");

            Console.ForegroundColor = level.ForegroundColor;
            Console.BackgroundColor = level.BackgroundColor;
            Console.Write(CenteredLogname(level));
            Console.ResetColor();

            Console.Write($"> {Message}\n");
            if(exception != null)
            {
                Console.WriteLine($"{exception.ToString()} - {exception.Message}");
                Console.WriteLine(exception.StackTrace);
            }
        }

        protected void writeLogToFile(string Timestamp, LogLevel level, string Message, Exception exception)
        {
            if(logFile == string.Empty || logFile == null)
                return;
            string LineToWrite = $"[{Timestamp}] {LoggerName} <{CenteredLogname(level)}> {Message}\n";
            using(var file = File.Open(logFile, FileMode.Append))
            {
                file.Write(System.Text.Encoding.UTF8.GetBytes(LineToWrite));
                if(exception != null)
                {
                    file.Write(System.Text.Encoding.UTF8.GetBytes($"{exception.ToString()} - {exception.Message}\n"));
                    file.Write(System.Text.Encoding.UTF8.GetBytes($"{exception.StackTrace}\n"));
                }
            }
        }

        #region Typesafe Log Methods
        public void Trace   (string Message, Exception e = null) => Log(LogLevel.Trace, Message, e);
        public void Verbose (string Message, Exception e = null) => Log(LogLevel.Verbose, Message, e);
        public void Debug   (string Message, Exception e = null) => Log(LogLevel.Debug, Message, e);
        public void Warn    (string Message, Exception e = null) => Log(LogLevel.Warning, Message, e);
        public void Info    (string Message, Exception e = null) => Log(LogLevel.Information, Message, e);
        public void Start   (string Message, Exception e = null) => Log(LogLevel.Start, Message, e);
        public void Done    (string Message, Exception e = null) => Log(LogLevel.Done, Message, e);
        public void Notice  (string Message, Exception e = null) => Log(LogLevel.Notice, Message, e);
        public void Alert   (string Message, Exception e = null) => Log(LogLevel.Alert, Message, e);
        public void Error   (string Message, Exception e = null) => Log(LogLevel.Error, Message, e);
        public void Fatal   (string Message, Exception e = null) => Log(LogLevel.Fatal, Message, e);
        #endregion
    }
}