using System;
using System.Collections.Generic;
using System.Text;

namespace DrrrAsync.Bot
{
    public class CommandAttribute : Attribute
    {
        public readonly string Name;
        public string Description = "";
        public string[] Aliases = new string[] { };
        public CommandAuthority Authority = CommandAuthority.User;

        public string StringSeperator = "\"";
        public bool ParseStrings = true;

        public CommandAttribute(string name) =>
            Name = name;
    }
}
