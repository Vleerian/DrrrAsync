using System;

namespace DrrrAsyncBot.Logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string Message, Exception exception = null);
    }
}