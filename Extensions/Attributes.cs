using System;
using System.Collections.Generic;
using System.Linq;

namespace DrrrAsync.Extensions
{
    namespace Attributes
    {
        /// <summary>
        /// The attribute the bot framework uses to get the command name, and check if it is, indeed, a command.
        /// </summary>
        public class CommandAttribute : Attribute
        {
            public string CommandName;

            public CommandAttribute(string aCommandName) =>
                CommandName = aCommandName;
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
            public List<string> Aliases;

            public AliasesAttribute(params string[] aAlias) =>
                Aliases = aAlias.ToList();
        }

        /// <summary>
        /// Commands with this attribute may only be used by the bot's owner
        /// </summary>
        public class RequiresOwnerAttribute : Attribute { }

        /// <summary>
        /// Commands with this attribute may only be used by 'elevated' individuals (Admins/Mods).
        /// </summary>
        public class RequiresElevatedAttribute : Attribute { }

        public class RemainderAttribute : Attribute { }
    }
}
