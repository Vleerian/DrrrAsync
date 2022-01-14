using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrrrAsync.Helpers;

namespace DrrrAsync.Core
{
    public partial class Bot
    {
        /// <summary>
        /// The function that runs when a command is executed
        /// </summary>
        /// <param name="Sender">The object which sent the event - usually Bot</param>
        /// <param name="e">The command message</param>
        /// <returns></returns>
        public async void ProcCommands(object Sender, AsyncMessageEvent e)
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

                Logger.Info($"{Message.Author.Name} executing {Command}...");

                //Check for the command or aliases.
                if (Commands.ContainsKey(Command))
                {
                    var cmdObject = Commands[Command];

                    if(!CheckPerms(User, cmdObject.Permission))
                    {
                        Logger.Alert("Insufficient permissions.");
                        return;
                    }

                    //Execute the command
                    try
                    {
                        await cmdObject.Call(new object[] { new Context(this, Message), CommandParams.ToArray() });
                    }
                    catch (Exception err)
                    {
                        Logger.Error("Command error.", err);
                    }
                    return;
                }
                //Log if the command doesn't exist
                Logger.Info("Command does not exist.");
            }
        }

        /// <summary>
        /// Prints messages onto the terminal
        /// </summary>
        /// <param name="Sender">The object which sent the event - usually Bot</param>
        /// <param name="e">The message</param>
        /// <returns></returns>
        public async void PrintMessage(object Sender, AsyncMessageEvent e)
        {
            var Client = (Bot)Sender;
            var Message = e.Message;

            string Timestamp = $"[{DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss")}]";
            string ClientStr = $"{{{Client.Name}-{Client.Room.Name}}}";
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
                    Mesg = $"{(Message.Secret ? " DIRECT " : " ")}{AuthorBit}: {Message.Text}";
                    Mesg += $" ({Message.Url})" ?? "";
                    break;
                case "me":
                    Mesg = $"{AuthorBit}: {Message.Author.Name}{Message.Content}";
                    break;
                case "roll":
                    Mesg = $"{AuthorBit} Rolled {TargetBit}";
                    break;
                case "music":
                    Mesg = $"MUSIC - {AuthorBit} shared a song.";
                    break;
                case "kick":
                case "ban":
                    Mesg = $"{Message.Type.ID.ToUpper()}ED - {TargetBit}";
                    break;
                case "join":
                case "leave":
                    Mesg = $"{Message.Type.ID.ToUpper()} - {AuthorBit}";
                    break;
                case "room-profile":
                    Mesg = $"Room updated.";
                    break;
                case "new-host":
                    Mesg = $"HANDOVER - {AuthorBit}";
                    break;
                case "system":
                    Mesg = $"<   SYSTEM    > - {Message.Text}";
                    break;
                default:
                    Mesg = $"[{Message.Type}] {Message.Text}";
                    break;
            }

            Console.WriteLine($"{Timestamp} {ClientStr} {Mesg}");

            if (!Directory.Exists("./Logs"))
                Directory.CreateDirectory("./Logs");
            File.AppendAllText($"./Logs/{base.Room.Name}_Log.txt", $"{Mesg}\n");
            await Task.CompletedTask;
        }
    }
}
