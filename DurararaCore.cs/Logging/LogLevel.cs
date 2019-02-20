using System;

namespace DrrrAsync.Logging
{
    public sealed class LogLevel
    {
        // "Enums" (Initializer List => no need for constructor)
        public static readonly LogLevel NONE = new LogLevel { Level = 0, Name = "None", Color = ConsoleColor.White };
        public static readonly LogLevel FATAL = new LogLevel { Level = 1, Name = "Fatal", Color = ConsoleColor.Magenta };
        public static readonly LogLevel ERROR = new LogLevel { Level = 2, Name = "Error", Color = ConsoleColor.Red };
        public static readonly LogLevel WARN = new LogLevel { Level = 3, Name = "Warning", Color = ConsoleColor.Yellow };
        public static readonly LogLevel INFO = new LogLevel { Level = 4, Name = "Info", Color = ConsoleColor.Green };
        public static readonly LogLevel DEBUG = new LogLevel { Level = 5, Name = "Debug", Color = ConsoleColor.Cyan };
        public static readonly LogLevel[] Levels = { NONE, FATAL, ERROR, WARN, INFO, DEBUG };

        // "Enum" Properties (Private set => settable only in own initializers)
        public int Level { get; private set; }
        public string Name { get; private set; }
        public ConsoleColor Color { get; private set; }

        // "Enums" implicitly castable to int (Like an actual Enum)
        public static implicit operator int(LogLevel level) { return level.Level; }
        public static implicit operator LogLevel(int i)
        {
            if (i < Levels.Length) return Levels[i];
            else throw (new Exception($"Invalid Conversion to LogLevel from {i}. The highest possible Level is 5 (Debug)."));
        }

        // Impossible to create new "Enums" (Like an actual Enum)
        private LogLevel() { }
    }
}
