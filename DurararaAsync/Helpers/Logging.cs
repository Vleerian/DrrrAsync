using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DrrrAsyncBot.Helpers;

using Console = Colorful.Console;

namespace DrrrAsyncBot.Helpers
{
    public enum LogEventType
    {
        Fatal,
        Error,
        Warning,
        Information,
        Debug,
    }

    public interface ILogger
    {
        void Log(LogEventType logLevel, string Message, Exception exception = null);
    }

    public class DefaultLogger : ILogger
    {
        public LogEventType logLevel;

        public void Log(LogEventType logEventType, string Message, Exception exception = null)
        {
            if (logEventType < logLevel)
                return;

            Color statusColor;
            switch (logEventType)
            {
                case LogEventType.Fatal:
                case LogEventType.Error:
                    statusColor = Color.Red;
                    break;
                case LogEventType.Warning:
                    statusColor = Color.Yellow;
                    break;
                case LogEventType.Information:
                    statusColor = Color.Cyan;
                    break;
                case LogEventType.Debug:
                    statusColor = Color.Orange;
                    break;
            }

            Console.WriteLineFormatted($"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}] <{{0}}> {Message}", statusColor, Color.White, logEventType.ToString().Center(9));
        }
    }
}
