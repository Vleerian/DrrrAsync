using System;

namespace DrrrAsync.Logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string Message, Exception exception = null);

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