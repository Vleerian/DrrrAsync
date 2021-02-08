using System;

namespace DrrrAsyncBot.Core
{
    /// <summary>
    /// The attribute added to a module to 'group' all it's commands
    /// Commands in groups are executed with the group's name leading
    /// I.E
    /// CommandName
    /// becomes
    /// GroupName CommandName
    /// </summary>
    public class GroupAttribute : Attribute
    {
        public string Name;

        public GroupAttribute(string name) =>
            Name = name;
    }

    /// <summary>
    /// The attribute the bot framework uses to get the command name, and check if it is, indeed, a command.
    /// </summary>
    public class CommandAttribute : Attribute
    {
        public string Name;

        public CommandAttribute(string name) =>
            Name = name;
    }

    /// <summary>
    /// The attribute the bot uses to get the command's documentation/description
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        public string Text;

        public DescriptionAttribute(string text) =>
            Text = text;
    }

    /// <summary>
    /// The attribute the bot uses to get the command's aliases.
    /// </summary>
    public class AliasesAttribute : Attribute
    {
        public string[] Aliases;

        public AliasesAttribute(params string[] aliases) =>
            Aliases = aliases;
    }
}