using System;
using System.Collections.Generic;
using System.Text;

namespace DrrrAsync
{
    namespace Extensions
    {
        namespace Attributes
        {
            class Command : Attribute
            {
                public string CommandName;

                public Command(string aCommandName) =>
                    CommandName = aCommandName;
            }

            class Description : Attribute
            {
                public string CommandDescription;

                public Description(string aCommandDescription) =>
                    CommandDescription = aCommandDescription;
            }

            class Aliases : Attribute
            {
                public string[] AliasList;

                public Aliases(params string[] aAlias) =>
                    AliasList = aAlias;
            }

            class RequiresOwner : Attribute { }

            class RequiresElevated : Attribute { }
        }
    }
}
