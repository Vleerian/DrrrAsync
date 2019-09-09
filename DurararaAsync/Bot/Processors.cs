using DrrrAsyncBot.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrrrAsyncBot.BotExtensions
{
    public struct CommandEvent
    {
        public Context Context;
        public string CommandName;
        public Command Command;
        public string[] Args;
        public bool Execute;
    }

    public interface ICommandProcessor
    {
        void Preprocess(ref CommandEvent command);
        void Postproceess(CommandEvent command);
    }
}
