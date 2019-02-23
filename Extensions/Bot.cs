using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;
using DrrrAsync.Logging;
using System.Linq;
using DrrrAsync.Extensions.Attributes;
using DrrrAsync.Bot;

namespace DrrrAsync.Extensions
{
    public partial class Bot : DrrrClient
    {
        private Dictionary<string, Command> Commands;

        public bool Running { get; private set; }

        public string CommandPrefix;

        public Stack<Tuple<string, string, string>> MessageQueue;

        /// <summary>
        /// The bot constructor instantiates the client, as well as registering it's command processor as an event.
        /// </summary>
        /// <param name="aName">The name the bot will use on Drrr.com</param>
        /// <param name="aIcon">The icon the bot will use on Drrr.com</param>
        public Bot() : base()
        {
            OnMessage += LookForCommands;
            Commands = new Dictionary<string, Command>();
            MessageQueue = new Stack<Tuple<string, string, string>>();
        }

        public void Shutdown() =>
            Running = false;

        /// <summary>
        /// The register command goes through all the methods in command module, and those with the Command attribute
        /// are added to the CommandDictionary.
        /// </summary>
        /// <typeparam name="T">The CommandModule's type</typeparam>
        public void Register<T>() where T : class
        {
            //Get the group, if any
            
            var Group = typeof(T).GetCustomAttribute<GroupAttribute>();
            string GroupName = (Group != null)? $"{Group.Name}":"";

            dynamic instance = Activator.CreateInstance<T>();
            
            foreach (var method in typeof(T).GetMethods())
            {
                if (method.Has(out DrrrAsync.Bot.CommandAttribute attribute))
                {
                    var command = new Command(instance,
                        method,
                        attribute.Name,
                        (method.Has(out DescriptionAttribute desc)) ? desc.Text : "",
                        GroupName
                    );

                    // Add the command and, if available, its Aliases to the List
                    Commands.Add(attribute.Name, command);

                    AliasesAttribute aliases;
                    if (method.Has(out aliases))
                        foreach (var alias in aliases.Aliases)
                            Commands.Add(alias, command);
                }
            }
        }

        /// <summary>
        /// The bot's command processor runs whenever a DrrrMessage event is thrown, and looks for commands.
        /// </summary>
        /// <param name="e">The DrrrMessage event.</param>
        public async Task LookForCommands(object sender, DrrrMessage message)
        {
            // Check if the message starts with the prefix.
            if (message.Text.StartsWith(CommandPrefix))
            {
                List<string> cmdParams = message.Text.Contains(' ')
                    ? message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>() { message.Text };

                string cmd = cmdParams.Pop(0).ToLower().Substring(CommandPrefix.Length);

                // If the command is a registered command, alias, or group, find it
                Command cmdToExecute = Commands.Single(item => item.Key == cmd || (item.Value.Group == cmd && item.Value.Name == cmdParams[0])).Value;

                //If something was found, execute it
                if (cmdToExecute != null)
                {
                    // Do a check for groups
                    if (cmd == cmdToExecute.Group)
                        cmd = cmdParams.Pop(0);

                    // Parameter and Arguments List
                    var parameters = cmdToExecute.Method.GetParameters();
                    var args = new List<object>() { new Context(this, message, message.From ?? message.Usr, message.PostedIn) };

                    // Iterate through the commands' required Parameters
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var paramType = parameters[i].ParameterType;
                        if (paramType != typeof(Context))
                        {
                            //Add the arguments to the list
                            try
                            {
                                //If the parameter takes the remaining values, let it be
                                if (paramType.Has<RemainingAttribute>())
                                {
                                    //Remaining only supports string.
                                    if (paramType != typeof(string))
                                        throw new ArgumentException("Remaining only supports string.");
                                    args.Add(string.Join(" ", cmdParams[i].Skip(i)));
                                    break;
                                }
                                else
                                    args.Add(Convert.ChangeType(cmdParams[i], paramType));
                            }
                            catch (Exception)
                            {
                                Logger.Error("Error with argument conversion.");
                            }
                        }
                    }

                    // Run the command and catch exceptions
                    try
                    {
                        await Commands[cmd].Call(args.ToArray());
                    }
                    catch (Exception err)
                    {
                        Logger.Error($"Command [{cmd}] errored with message: {err.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Starts the bot's primary loop. It takes a room name, and will attempt to join it.
        /// </summary>
        /// <param name="roomName">The name of the room you want to join</param>
        /// <returns>True if the bot started successfully, false otherwise.</returns>
        public async Task<bool> Connect(string roomName)
        {
            List<DrrrRoom> Rooms = await GetLounge();
            DrrrRoom Room = Rooms.Find(lRoom => lRoom.Name == roomName);
            bool Connected = false;

            if (Room == null)
                Logger.Error("Failed to connect, room not found.");
            else if (Room.Full)
                Logger.Error("Failed to connect, room is full.");
            else if (Room.Users.Any(User => User.Name == Name))
                Logger.Error("User exists in room");
            else
                Connected = true;
            
            if (!Connected)
                return false;

            Logger.Info("Connecting...");
            return await Connect(Room);
        }

        /// <summary>
        /// Starts the bot's primary loop. It takes a room, and will attempt to join it.
        /// </summary>
        /// <param name="room">The room you want to join</param>
        /// <returns>True if the bot started successfully, false otherwise.</returns>
        public async Task<bool> Connect(DrrrRoom room)
        {
            //If the need arises, processing can be done on the resulting DrrrRoom object.
            await JoinRoom(room.RoomId);
            Logger.Info("Done...");

            if (!Running)
            {
                Running = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(() => BotMain());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            return true;
        }

        /// <summary>
        /// Queues a message to be sent.
        /// </summary>
        /// <param name="Message">The message you want to send</param>
        /// <param name="Url">An optional URL to send</param>
        /// <param name="To">An optional user to send it to</param>
        /// <returns>Will not push anything to the queue if the user isn't in the room, or if you're not in a room.</returns>
        public new async Task SendMessage(string Message, string Url = "", string To = null)
        {
            //Do some checks to make sure a message can actually be sent.
            if (Room == null)
                return;
            else if (To != null && !Room.Users.Any(User => User.Name == To))
                    return;
            //Push it to the message queue
            MessageQueue.Push(new Tuple<string, string, string>(Message, Url, To));
            await Task.CompletedTask; //Make the warning go away.
        }

        /// <summary>
        /// The message loop. This prevents multiple messages from being sent all at once.
        /// It also prevents the bot from timing out
        /// </summary>
        private async Task MessageLoop()
        {
            Logger.Debug("Starting message loop.");
            DateTime LastMessage = DateTime.Now;
            while (Running)
            {
                await Task.Delay(250);
                if (Room != null && MessageQueue.Count > 0)
                {
                    Tuple<string, string, string> Msg = MessageQueue.Pop();
                    await base.SendMessage(Msg.Item1, Msg.Item2, Msg.Item3);
                    LastMessage = DateTime.Now;
                }

                if ((DateTime.Now - LastMessage).TotalSeconds > (10 * 60))
                {
                    await base.SendMessage("[HEARTBEAT]", To: User.Name);
                    LastMessage = DateTime.Now;
                }
                    
            }
        }

        /// <summary>
        /// The bot's primary loop.
        /// </summary>
        private async Task BotMain()
        {
            Logger.Debug("Starting loop.");
#pragma warning disable CS4014
            MessageLoop();
#pragma warning restore CS4014
            while (Running)
            {
                DrrrRoom Data = await GetRoom();

                await Task.Delay(250);
            }

            Logger.Info("Loop ended.");
        }
    }
}
