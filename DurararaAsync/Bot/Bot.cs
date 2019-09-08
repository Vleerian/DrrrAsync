using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DrrrBot.Helpers;
using DrrrBot.Objects;
using DrrrBot.Permission;

using Console = Colorful.Console;
using Newtonsoft.Json;

namespace DrrrBot.Core
{
    public class Bot : DrrrClient
    {
        private Dictionary<string, Command> Commands;
        private Queue<DrrrMessageConfig> MessageQueue;

        private bool running;
        public bool Running
        {
            get
            {
                return running;
            }
        }

        public DrrrBotConfig Config { get; private set; }

        private Bot(DrrrBotConfig config)
        {
            Config = config;
            Commands = new Dictionary<string, Command>();
            MessageQueue = new Queue<DrrrMessageConfig>();
        }

        public async Task ReloadCongfig(string ConfigPath)
        {
            var json = "";
            using (var fs = File.OpenRead(ConfigPath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Config = JsonConvert.DeserializeObject<DrrrBotConfig>(json);
        }

        /// <summary>
        /// Uses a DrrrBotConfig object to create a bot.
        /// </summary>
        /// <param name="Config">See: DrrrBotConfig</param>
        /// <returns>An instance of Bot</returns>
        public static Bot Create(DrrrBotConfig Config)
        {
            if (Config.Name == null || Config.Name == "")
                throw new Exception("No Name.");
            else if (Config.Room.Name == null || Config.Room.Name == "")
                throw new Exception("No Room Name.");
            else if (Config.CommandSignal == null || Config.CommandSignal == "")
                throw new Exception("No Command Signal.");
            else if (Config.Icon == null)
                throw new Exception("No Icon.");

            return new Bot(Config);
        }

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

        /// <summary>
        /// Hides the base send message, instead adding a message to the queue to be sent.
        /// </summary>
        /// <param name="aMessage">The body of the message being sent</param>
        /// <param name="aUsername">Optional. The user to send a direct message to.</param>
        /// <param name="aUrl">Optional. a URL to attach to the message.</param>
        /// <returns></returns>
        public async new Task SendMessage(string aMessage, string aUsername = "", string aUrl = "")
        {
            MessageQueue.Enqueue(new DrrrMessageConfig()
            {
                Message = aMessage,
                Username = aUsername,
                Url = aUrl,
                Direct = false
            });
            await Task.CompletedTask;
        }

        /// <summary>
        /// Adds a direct message to be added.
        /// </summary>
        /// <param name="aMessage">The body of the message being sent</param>
        /// <param name="aUsername">Optional. The user to send a direct message to.</param>
        /// <param name="aUrl">Optional. a URL to attach to the message.</param>
        /// <returns></returns>
        public async Task SendDirectMessage(string aMessage, string aUsername = "", string aUrl = "")
        {
            MessageQueue.Enqueue(new DrrrMessageConfig()
            {
                Message = aMessage,
                Username = aUsername,
                Url = aUrl,
                Direct = true
            });
            await Task.CompletedTask;
        }

        /// <summary>
        /// The register command goes through all the methods in command module, and 
        /// those with the Command attribute are added to the CommandDictionary.
        /// </summary>
        /// <typeparam name="T">The CommandModule's type</typeparam>
        public async Task Register<T>() where T : class
        {
            //Get the group, if any
            dynamic instance = Activator.CreateInstance<T>();

            foreach (var method in typeof(T).GetMethods())
            {
                if (method.Has(out CommandAttribute attribute))
                {
                    var command = new Command(instance,
                        method,
                        attribute.Name,
                        (method.Has(out DescriptionAttribute desc)) ? desc.Text : "",
                        (method.Has(out RequiresPermission perm)) ? perm.Permission : 0
                    );

                    // Add the command and, if available, its Aliases to the List
                    Commands.Add(attribute.Name, command);

                    if (method.Has(out AliasesAttribute aliases))
                        foreach (var alias in aliases.Aliases)
                            Commands.Add(alias, command);
                }
            }
            await Task.CompletedTask;
        }

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

                Utilities.Log(Color.Aqua, "CMND", $"{Message.Author.Name} executing {Command}...");

                //Check for the command or aliases.
                if (Commands.ContainsKey(Command))
                {
                    //The command exists. Do preliminary permissions check
                    if(!CheckPerms(Message.Author, Commands[Command].Permission))
                    {
                        Utilities.Log(Color.Coral, "DENIED", "Insufficient permissions.");
                        return;
                    }
                    //Now that they've been cleared to run the command, work the arguments into an array, and execute it
                    try
                    {
                        var args = new List<object>() { new Context(this, Message), CommandParams.ToArray() };
                        await Commands[Command].Call(args.ToArray());
                    }
                    catch(Exception err)
                    {
                        Utilities.Log(Color.DarkOrange, "CMND ERR", err.Message);
                    }
                    return;
                }
                //Log if the command doesn't exist
                Utilities.Log(Color.Orange, "CMND", $"Command does not exist.");
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

        private async Task MessageLoop()
        {
            DateTime LastSent = DateTime.Now;
            while (Running)
            {
                await Task.Delay(800);
                if (!Running)
                    break;

                if (MessageQueue.Count > 0)
                {
                    var Message = MessageQueue.Dequeue();

                    if(Message.Username != "")
                    {
                        if (Room.TryGetUser(Message.Username, out DrrrUser User))
                            Message.Username = User.ID;
                        else
                        {
                            Utilities.Log(Color.Red, "ERR", "User is not in room.");
                            continue;
                        }
                    }

                    LastSent = DateTime.Now;
                    await base.SendMessage(Message.Message, Message.Url, Message.Username);
                }
                if ((DateTime.Now - LastSent).TotalMinutes >= 15)
                    await SendMessage("[HEARTBEAT]", Name);
            }
        }

        public async Task Reconnect()
        {
            Utilities.Log(Color.Cyan, "INFO", "Attempting to reconnect.");

            Utilities.Log(Color.Cyan, "INFO", "Shutting down all loops...");
            running = false;

            Utilities.Log(Color.Yellow, "NETWORK", "Retrieving roomlist.");
            var roomList = await GetLounge();
            Utilities.Log(Color.Green, "OK", "Roomlist retrieved.");

            Utilities.Log(Color.Cyan, "INFO", "Searching for target room...");
            DrrrRoom Found = roomList.Find(Room => Room.Name == Config.Room.Name);
            if (Found == null || Found.Users.Find(User => User.Name == "Welne Zodiv") == null)
            {
                Utilities.Log(Color.Orange, "NOTICE", "Reconnect impossible. Attempting rejoin.");
                await Run();
                return;
            }
            Utilities.Log(Color.Green, "SUCCESS", "Reconnect possible.");

            //Print the room's history, if joining the room
            var JoinMessages = await GetRoom();
            foreach (var Message in JoinMessages.Messages)
                await PrintMessage(this, new AsyncMessageEvent(Message));

            running = true;

#pragma warning disable CS4014 // This is starting the message loop, and we don't particularly care if it runs away.
            MessageLoop();
#pragma warning restore CS4014

            while (Running)
            {
                //I do it this way so that if you press Q during the delay, you get a quicker
                for (int i = 0; i < 50; i++)
                {
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        Utilities.Log(Color.Cyan, "INFO", "Shutdown requested from terminal.");
                        Shutdown();
                    }
                    await Task.Delay(10);
                }
                if (!Running)
                    break;

                foreach (var Message in await GetRoomUpdate())
                {
                    if (Message.Secret)
                        await On_Direct_Message?.InvokeAsync(this, new AsyncMessageEvent(Message));
                    await On_Message?.InvokeAsync(this, new AsyncMessageEvent(Message));
                }
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Utilities.Log(Color.Orange, "CLEANUP", "Leaving room...");
            foreach (var User in Room.Users)
            {
                if (User.Tripcode != null && Config.Permissions.ContainsKey(User.Tripcode))
                    await GiveHost(User);
            }
            if (await LeaveRoom() == "Cannot leave.")
            {
                Utilities.Log(Color.Yellow, "WARNING", "Cannot leave room. Waiting 40s and trying again.");
                await Task.Delay(40000);
                if (await LeaveRoom() == "Cannot leave.")
                    Utilities.Log(Color.Yellow, "WARNING", "Cannot leave room. Proceeding with cleanup anyway.");
                else
                    Utilities.Log(Color.Green, "OK", "Room left.");

            }
            else
                Utilities.Log(Color.Green, "OK", "Room left.");

            Utilities.Log(Color.Cyan, "INFO", "Exited.");
            Console.ReadKey();
        }

        public async Task Run()
        {
            Name = Config.Name;
            Utilities.Log(Color.Yellow, "NETWORK", "Logging in.");
            await Login();
            Utilities.Log(Color.Green, "OK", "Logged in.");

            Utilities.Log(Color.Yellow, "NETWORK", "Retrieving roomlist.");
            var roomList = await GetLounge();
            Utilities.Log(Color.Green, "OK", "Roomlist retrieved.");

            Utilities.Log(Color.Cyan, "INFO", "Searching for target room...");
            DrrrRoom Found = roomList.Find(Room => Room.Name == Config.Room.Name);
            try
            {
                if (Found == null)
                {
                    Utilities.Log(Color.Orange, "NOTICE", "Room, not found.");
                    if(Config.Room.Description != null)
                    {
                        Utilities.Log(Color.Yellow, "NETWORK", "Creating room.");
                        await MakeRoom(Config.Room);
                    }
                }
                else if (Found.UserCount >= Found.Limit)
                    throw new Exception("Room full.");
                else if (Found.Users.Find(User => User.Name == "Welne Zodiv") != null)
                    throw new Exception("User exists in room.");
            }
            catch (Exception e)
            {
                Utilities.Log(Color.Red, "ERROR", e.Message);
                Console.ReadKey();
                return;
            }

            On_Post.Register(PrintMessage);
            On_Message.Register(ProcCommands);
            On_Direct_Message.Register(ProcCommands);

            Utilities.Log(Color.Green, "OK", "Room exists.");
            if (Found != null)
            {
                Utilities.Log(Color.Cyan, "INFO", $"{Found.Name} - {Found.UserCount}/{Found.Limit}");
                await JoinRoom(Found.RoomId);
            }

            //Print the room's history, if joining the room
            var JoinMessages = await GetRoom();
            foreach (var Message in JoinMessages.Messages)
                await PrintMessage(this, new AsyncMessageEvent(Message));

            running = true;

#pragma warning disable CS4014 // This is starting the message loop, and we don't particularly care if it runs away.
            MessageLoop();
#pragma warning restore CS4014

            while (Running)
            {
                //I do it this way so that if you press Q during the delay, you get an instant response
                for (int i = 0; i < 50; i++)
                {
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        Utilities.Log(Color.Cyan, "INFO", "Shutdown requested from terminal.");
                        Shutdown();
                    }
                    await Task.Delay(10);
                }
                if (!Running)
                    break;

                foreach (var Message in await GetRoomUpdate())
                {
                    if(Message.Type == DrrrMessageType.Message)
                    {
                        if (Message.Secret)
                            await On_Direct_Message?.InvokeAsync(this, new AsyncMessageEvent(Message));
                        else
                            await On_Message?.InvokeAsync(this, new AsyncMessageEvent(Message));
                    }
                    if (Message.Type == DrrrMessageType.Join)
                        await On_Join?.InvokeAsync(this, new AsyncMessageEvent(Message));

                    await On_Post?.InvokeAsync(this, new AsyncMessageEvent(Message));
                }
            }

            Console.SetCursorPosition(0, Console.CursorTop);
            Utilities.Log(Color.Orange, "CLEANUP", "Leaving room...");

            try
            {
                foreach (var User in Room.Users)
                {
                    if (User.Tripcode != null && Config.Permissions.ContainsKey(User.Tripcode))
                        await GiveHost(User);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e} --- {e.Message}");
                Console.WriteLine(e.StackTrace);
                Utilities.Log(Color.Red, "ERROR", "Could not handover host. Attempting to continue shutdown.");
            }
            
            if (await LeaveRoom() == "Cannot leave.")
            {
                Utilities.Log(Color.Yellow, "WARNING", "Cannot leave room. Waiting 40s and trying again.");
                await Task.Delay(40000);
                if (await LeaveRoom() == "Cannot leave.")
                    Utilities.Log(Color.Yellow, "ERROR", "Failed to leave room. Proceeding with shutdown anyway.");
                else
                    Utilities.Log(Color.Green, "OK", "Room left.");
            }
            else
                Utilities.Log(Color.Green, "OK", "Room left.");

            Utilities.Log(Color.Cyan, "INFO", "Exited.");
        }

        public void Shutdown()
        {
            running = false;
        }
    }
}
