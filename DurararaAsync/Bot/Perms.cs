using DrrrBot.Objects;
using DrrrBot.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrrrBot.Permission
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

    public enum PermLevel {
        None = 0,
        Moderator = 1,
        Admin = 2,
        Owner = 3
    }
}
