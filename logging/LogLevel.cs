using System;

namespace DrrrAsync.Logging
{
    public class LogLevel
    {
        #region Variable Declarations
        public readonly string ShortName;
        public readonly string LongName;
        public readonly int Level;
        
        public readonly ConsoleColor ForegroundColor;
        public readonly ConsoleColor BackgroundColor;
        #endregion

        #region Constructors
        public LogLevel(int level, string shortName, string longName)
        {
            if(shortName == null)
                throw new ArgumentNullException("shortName");
            if(longName == null)
                throw new ArgumentException("longname");

            Level = level;
            ShortName = shortName;
            LongName = longName;

            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }

        public LogLevel(int level, string shortName, string longName, ConsoleColor FG, ConsoleColor BG)
        {
            if(shortName == null)
                throw new ArgumentNullException("shortName");
            if(longName == null)
                throw new ArgumentException("longname");

            Level = level;
            ShortName = shortName;
            LongName = longName;

            ForegroundColor = FG;
            BackgroundColor = BG;
        }
        #endregion

        #region Operator Overides
        public override int GetHashCode() => Level;

        public override bool Equals(object o)
        {
            LogLevel otherLevel = o as LogLevel;
            if(otherLevel != null)
                return Level == otherLevel.Level;
            return base.Equals(o);
        }

        public static bool operator == (LogLevel left, LogLevel right)
        {
            object lo = left as object;
            object ro = right as object;
            if(ro != null && lo != null)
                return left.Level == right.Level;
            return lo == ro;
        }

        public static bool operator != (LogLevel left, LogLevel right) => !(left == right);        

        public static bool operator > (LogLevel left, LogLevel right) => left.Level > right.Level;
        
        public static bool operator < (LogLevel left, LogLevel right) => left.Level < right.Level;

        public static bool operator >= (LogLevel left, LogLevel right) => left.Level >= right.Level;
        
        public static bool operator <= (LogLevel left, LogLevel right) => left.Level <= right.Level;
        #endregion

        #region Typesafe Enums
        public static readonly LogLevel Off         = new (int.MaxValue, "OFF", "Off");
        public static readonly LogLevel All         = new (int.MinValue, "ALL", "All");
        public static readonly LogLevel Trace       = new (0, "TRACE", "Trace");
        public static readonly LogLevel Verbose     = new (500, "VERB", "Verbose");
        public static readonly LogLevel Debug       = new (1000, "DEBUG", "Debug");
        public static readonly LogLevel Warning     = new (2000, "WARN", "Warning");
        public static readonly LogLevel Information = new (2500, "INFO", "Information");
        public static readonly LogLevel Start       = new (2500, "START", "Start");
        public static readonly LogLevel Done        = new (2500, "DONE", "Done");
        public static readonly LogLevel Notice      = new (3000, "NOTE", "Notice");
        public static readonly LogLevel Alert       = new (3000, "ALERT", "Alert");
        public static readonly LogLevel Error       = new (4000, "ERROR", "Error");
        public static readonly LogLevel Fatal       = new (5000, "FATAL", "Fatal");
        #endregion
    }
}