using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colorful;
using DrrrAsyncBot.BotExtensions;
using DrrrAsyncBot.Helpers;
using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Permission;

using Console = Colorful.Console;

namespace DrrrAsyncBot.Core
{
    public partial class Bot
    {
        /// <summary>
        /// The function that runs when a command is executed
        /// </summary>
        /// <param name="Sender">The object which sent the event - usually Bot</param>
        /// <param name="e">The command message</param>
        /// <returns></returns>
        public async Task ProcCommands(object Sender, AsyncMessageEvent e)
        {
            var Message = e.Message;
            //It has to be a message, and it has to start with the command signal
            if (Message.Text.StartsWith(Config.CommandSignal))
            {
                //Parse the message to get the command to execute, as well as any parameters to pass.
                List<string> CommandParams = Message.Text.Contains(" ") ?
                    Message.Text.Split(new char[] { ' ' }, options: StringSplitOptions.RemoveEmptyEntries).ToList() :
                    new List<string>() { Message.Text };
                string Command = CommandParams
                                    .Pop(0).ToLower()
                                    .Substring(Config.CommandSignal.Length);

                Logger.Log(LogEventType.Information, $"{Message.Author.Name} executing {Command}...");

                //Check for the command or aliases.
                if (Commands.ContainsKey(Command))
                {
                    var commandEvent = new CommandEvent()
                    {
                        Context = new Context(this, Message),
                        CommandName = Command,
                        Command = Commands[Command],
                        Args = CommandParams.ToArray(),
                        Execute = true
                    };

                    //Run preprocessors
                    commandProcessors.ForEach(C => C.Preprocess(ref commandEvent));

                    //Execute the command
                    try
                    {
                        if(commandEvent.Execute)
                        {
                            await Commands[commandEvent.CommandName].Call(new object[] { commandEvent.Context, commandEvent.Args });
                            commandProcessors.ForEach(C => C.Postproceess(commandEvent));
                        }
                    }
                    catch (Exception err)
                    {
                        Logger.Log(LogEventType.Error, "Command error.", err);
                    }
                    return;
                }
                //Log if the command doesn't exist
                Logger.Log(LogEventType.Information, "Command does not exist.");
            }
        }

        /// <summary>
        /// Prints messages onto the terminal
        /// </summary>
        /// <param name="Sender">The object which sent the event - usually Bot</param>
        /// <param name="e">The message</param>
        /// <returns></returns>
        public async Task PrintMessage(object Sender, AsyncMessageEvent e)
        {
            var Message = e.Message;
            StyleSheet MessageStyle = new StyleSheet(Color.White);
            MessageStyle.AddStyle("(?<=<).+?(?=#|>)", Color.Green);
            MessageStyle.AddStyle("(?<=#).*(?=>)", Color.Blue);
            MessageStyle.AddStyle("BANED|KICKED", Color.Red);
            MessageStyle.AddStyle("JOIN|LEAVE", Color.Orange);
            MessageStyle.AddStyle("DIRECT", Color.Magenta);
            MessageStyle.AddStyle("MUSIC", Color.Cyan);

            string Timestamp = $"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}]";

            string AuthorBit = "";
            if (Message.Author != null)
                AuthorBit = Message.Author.Tripcode != null ? $"<{Message.Author.Name}#{Message.Author.Tripcode}>" : $"<{Message.Author.Name}>";
            string TargetBit = "";
            if (Message.Target != null)
                TargetBit = Message.Target.Tripcode != null ? $"<{Message.Target.Name}#{Message.Target.Tripcode}>" : $"<{Message.Target.Name}>";

            string Mesg;
            switch (Message.Type)
            {
                case "message":
                    Mesg = $"{Timestamp}{(Message.Secret ? " DIRECT " : " ")}{AuthorBit}: {Message.Text}";
                    Mesg += $" ({Message.Url})" ?? "";
                    break;
                case "me":
                    Mesg = $"{Timestamp} {AuthorBit}: {Message.Author.Name}{Message.Content}";
                    break;
                case "roll":
                    Mesg = $"{Timestamp} {AuthorBit} Rolled {TargetBit}";
                    break;
                case "music":
                    Mesg = $"{Timestamp} MUSIC - {AuthorBit} shared a song.";
                    break;
                case "kick":
                case "ban":
                    Mesg = $"{Timestamp} {Message.Type.ID.ToUpper()}ED - {TargetBit}";
                    break;
                case "join":
                case "leave":
                    Mesg = $"{Timestamp} {Message.Type.ID.ToUpper()} - {AuthorBit}";
                    break;
                case "room-profile":
                    Mesg = $"{Timestamp} Room updated.";
                    break;
                case "new-host":
                    Mesg = $"{Timestamp} HANDOVER - {AuthorBit}";
                    break;
                case "system":
                    Mesg = $"{Timestamp} <   SYSTEM    > - {Message.Text}";
                    break;
                default:
                    Mesg = $"[{Message.Type}] {Message.Text}";
                    break;
            }

            Console.WriteLineStyled(Mesg, MessageStyle);

            if (!Directory.Exists("./Logs"))
                Directory.CreateDirectory("./Logs");
            File.AppendAllText($"./Logs/{base.Room.Name}_Log.txt", $"{Mesg}\n");
            await Task.CompletedTask;
        }
    }
}
