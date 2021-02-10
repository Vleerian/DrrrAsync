using System;
using System.Collections.Generic;
using System.Text;

using DrrrAsyncBot.Helpers;
using DrrrAsyncBot.Logging;
using DrrrAsyncBot.Core;
using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Permission;
using DrrrAsyncBot.BotExtensions;

namespace DrrrAsyncBot.Core
{
    public partial class Bot
    {
        /// <summary>
        /// Check if a user is permitted to execute a command
        /// </summary>
        /// <param name="user">The user you want to check</param>
        /// <param name="aPermission">The permisison level you want them to check against</param>
        /// <returns>True if they pass, false if they don't.</returns>
        public bool CheckPerms(DrrrUser user, PermLevel aPermission)
        {
            //Don't bother permission checking if anyone should be able to run it
            if (aPermission == 0)
                return true;

            //Make sure the user CAN have permissions, and that they actually do
            if (user.Tripcode == null || !Config.Permissions.ContainsKey(user.Tripcode))
                return false;
            PermLevel User_Permission = Config.Permissions[user.Tripcode];

            //Lower permission level = less permission. Therefore, if you permission lever is greater or equal to
            //the command's permission level, you are allowed to execute it.
            if (User_Permission >= aPermission)
                return true;
            return false;
        }
    }
}

namespace DrrrAsyncBot.Permission
{
    
    /// <summary>
    /// Commands with this attribute may only be used by the bot's owner
    /// </summary>
    public class RequiresPermission : Attribute
    {
        private PermLevel permission_;
        public PermLevel Permission { get => permission_; }

        public RequiresPermission(PermLevel aPerm) =>
            permission_ = aPerm;

    }

    class PermissionsProcessor : ICommandProcessor
    {
        public void Postproceess(CommandEvent command) { }

        public void Preprocess(ref CommandEvent command)
        {
            Bot client = command.Context.Client;
            if(!client.CheckPerms(command.Context.Author, command.Command.Permission))
            {
                BasicLogger.Default.Info("Insufficient permissions.");
                command.Execute = false;
            }
        }
    }

    public enum PermLevel {
        None = 0,
        Trusted = 1,
        Moderator = 2,
        Admin = 3,
        Operator = 4,
        Owner = 5
    }
}
