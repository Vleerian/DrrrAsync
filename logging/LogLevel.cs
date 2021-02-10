using System;

namespace DrrrAsyncBot.Logging
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
        public static readonly LogLevel Trace       = new (0, "TRACE", "Trace", ConsoleColor.White, ConsoleColor.Black);
        public static readonly LogLevel Verbose     = new (500, "VERB", "Verbose", ConsoleColor.White, ConsoleColor.Black);
        public static readonly LogLevel Debug       = new (1000, "DEBUG", "Debug", ConsoleColor.Magenta, ConsoleColor.Black);
        public static readonly LogLevel Warning     = new (2000, "WARN", "Warning", ConsoleColor.Yellow, ConsoleColor.Black);
        public static readonly LogLevel Information = new (2500, "INFO", "Information", ConsoleColor.Cyan, ConsoleColor.Black);
        public static readonly LogLevel Start       = new (2500, "START", "Start", ConsoleColor.Green, ConsoleColor.Black);
        public static readonly LogLevel Done        = new (2500, "DONE", "Done", ConsoleColor.Green, ConsoleColor.Black);
        public static readonly LogLevel Notice      = new (3000, "NOTE", "Notice", ConsoleColor.Cyan, ConsoleColor.Black);
        public static readonly LogLevel Alert       = new (3000, "ALERT", "Alert", ConsoleColor.Black, ConsoleColor.Yellow);
        public static readonly LogLevel Error       = new (4000, "ERROR", "Error", ConsoleColor.Red, ConsoleColor.Black);
        public static readonly LogLevel Fatal       = new (5000, "FATAL", "Fatal", ConsoleColor.White, ConsoleColor.Red);
        public static readonly LogLevel NewRoom     = new (2500, "MAKE", "New Room", ConsoleColor.Green, ConsoleColor.Black);
        public static readonly LogLevel NewName     = new (2500, "NAME", "New Name", ConsoleColor.Green, ConsoleColor.Black);
        public static readonly LogLevel NewDesc     = new (2500, "DESC", "New Description", ConsoleColor.Cyan, ConsoleColor.Black);
        public static readonly LogLevel NewHost     = new (2500, "HOST", "New Host", ConsoleColor.Cyan, ConsoleColor.Black);
        public static readonly LogLevel Update      = new (2500, "EDIT", "New Host", ConsoleColor.Cyan, ConsoleColor.Black);
        public static readonly LogLevel Deleted     = new (2500, "DEAD", "Room Deleted", ConsoleColor.Magenta, ConsoleColor.Black);
        public static readonly LogLevel UserJoin    = new (2500, "JOIN", "User Joined", ConsoleColor.Green, ConsoleColor.Black);
        public static readonly LogLevel UserLeave   = new (2500, "LEFT", "User Left", ConsoleColor.Magenta, ConsoleColor.Black);
        #endregion
    }
}