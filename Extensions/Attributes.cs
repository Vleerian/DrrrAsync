using System;

namespace DrrrAsync
{
    namespace Extensions
    {
        namespace Attributes
        {
            /// <summary>
            /// The attribute the bot framework uses to get the command name, and check if it is, indeed, a command.
            /// </summary>
            class CommandAttribute : Attribute
            {
                public string CommandName;

                public CommandAttribute(string aCommandName) =>
                    CommandName = aCommandName;
            }

            /// <summary>
            /// The attribute the bot uses to get the command's documentation/description
            /// </summary>
            class AttributeDescription : Attribute
            {
                public string CommandDescription;

                public AttributeDescription(string aCommandDescription) =>
                    CommandDescription = aCommandDescription;
            }

            /// <summary>
            /// The attribute the bot uses to get the command's aliases.
            /// </summary>
            class AttributeAliases : Attribute
            {
                public string[] AliasList;

                public AttributeAliases(params string[] aAlias) =>
                    AliasList = aAlias;
            }

            /// <summary>
            /// Commands with this attribute may only be used by the bot's owner
            /// </summary>
            class AttributeRequiresOwner : Attribute { }

            /// <summary>
            /// Commands with this attribute may only be used by 'elevated' individuals (Admins/Mods).
            /// </summary>
            class AttributeRequiresElevated : Attribute { }
        }
    }
}
