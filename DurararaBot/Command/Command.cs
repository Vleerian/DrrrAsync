using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;

namespace DrrrAsync.Bot
{

    /// <summary>
    /// The command class is used for handling bot commands.
    /// It contains the command's name, description, as well as a
    /// reference to it's container module, and handler.
    /// </summary>
    public class Command
    {
        public readonly string Name;
        public readonly string Description;
        private readonly Delegate Handler;
        public Module Module;
        public readonly CommandAuthority Authority;

        public string StringSeperator;
        public bool ParseStrings;

        /// <summary>Executes the command.</summary>
        /// <param name="e">The event arguments, passed as a tuple.</param>
        public async Task Execute(object sender, string remaining, DrrrMessage message)
        {
            var paramTypes = Handler.GetMethodInfo().GetParameters();
            var parsedArgs = new List<object>();

            foreach(var paramType in paramTypes)
            {
                switch(paramType.GetType())
                {
                    case var tContext when tContext == typeof(Context):
                        parsedArgs.Add(new Context((DrrrBot)sender, message));
                        continue;
                    case var tUser when tUser == typeof(DrrrUser):
                        try { parsedArgs.Add(message.Room.Users.Where(x => remaining.StartsWith($"@{x.Name}")).SingleOrDefault()); }
                        catch (Exception e) { throw new Exception("Command argument of type User is invalid.", e); }
                        continue;
                    case var tString when tString == typeof(string):
                        string foundString;
                        if (ParseStrings)
                            foundString = remaining.Split(StringSeperator)[1];
                        else if (paramType == paramTypes.Last())
                            foundString = remaining;
                        else throw new Exception("Whoever made this command is invalid.");
                        parsedArgs.Add(foundString);
                        if (foundString.Length + StringSeperator.Length * 2 <= remaining.Length)
                            remaining = remaining.Substring(foundString.Length + StringSeperator.Length * 2);
                        else throw new Exception("Command argument of type String is invalid.");
                        continue;
                    case var type:
                        var arg = remaining.Split(' ')[0];
                        try { parsedArgs.Add(Convert.ChangeType(arg, type)); }
                        catch (Exception e) { throw new Exception("Command argument is invalid", e); }
                        remaining = remaining.Substring(arg.Length+1);
                        continue;
                }
            }

            Handler.DynamicInvoke(parsedArgs.ToArray());
        }

        public Command(Module module, Delegate handler, string name, string description, CommandAuthority authority, string stringSeperator, bool parseStrings)
        {
            Module = module;
            Handler = handler;
            Name = name;
            Description = description;
            Authority = authority;
            StringSeperator = stringSeperator;
            ParseStrings = parseStrings;
        }
    }
}
