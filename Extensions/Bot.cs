using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;
using DrrrAsync.Logging;
using System.Linq;
using DrrrAsync.Extensions.Attributes;

namespace DrrrAsync.Extensions
{
    public class Bot : DrrrClient
    {
        private Dictionary<string, Command> Commands;

        public bool Running { get; private set; }

        public string CommandPrefix;

        /// <summary>
        /// The bot constructor instantiates the client, as well as registering it's command processor as an event.
        /// </summary>
        /// <param name="aName">The name the bot will use on Drrr.com</param>
        /// <param name="aIcon">The icon the bot will use on Drrr.com</param>
        public Bot() : base()
        {
            OnMessage += LookForCommands;
            Commands = new Dictionary<string, Command>();
        }

        public void Shutdown() =>
            Running = false;

        /// <summary>
        /// The register command goes through all the methods in command module, and those with the Command attribute
        /// are added to the CommandDictionary.
        /// </summary>
        /// <typeparam name="T">The CommandModule's type</typeparam>
        /// <param name="CommandModule">The command module.</param>
        public void RegisterCommands<T>() where T : class
        {
            dynamic Instance = Activator.CreateInstance<T>();
            foreach (var Method in typeof(T).GetMethods())
            {
                if (Method.GetCustomAttribute<CommandAttribute>() is CommandAttribute attribute)
                {
                    var command = Method.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute desc
                        ? new Command(Instance, Method, attribute.CommandName, desc.Text)
                        : new Command(Instance, Method, attribute.CommandName, "");

                    // Add the command and, if available, its Aliases to the List
                    Commands.Add(attribute.CommandName, command);
                    if (Method.GetCustomAttribute<AliasesAttribute>() is AliasesAttribute aliases)
                        foreach (var alias in aliases.Aliases)
                            Commands.Add(alias, command);
                }
            }
        }

        /// <summary>
        /// The bot's command processor runs whenever a DrrrMessage event is thrown, and looks for commands.
        /// </summary>
        /// <param name="e">The DrrrMessage event.</param>
        private async Task LookForCommands(DrrrMessage message)
        {
            // Check if the message starts with the prefix.
            if (message.Text.StartsWith(CommandPrefix))
            {
                string[] cmdParams = message.Text.Contains(' ')
                    ? message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    : new string[] { message.Text };

                string cmd = cmdParams[0].ToLower().Substring(CommandPrefix.Length);

                // If the command is a registered command or alias, execute it
                if (Commands.ContainsKey(cmd))
                {
                    // Parameter and Arguments List
                    var parameters = Commands[cmd].Method.GetParameters();
                    var args = new List<object>();

                    // Iterate through the commands' required Parameters
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var paramType = parameters[i].ParameterType;
                        if (paramType == typeof(Context))
                            args.Add(new Context(this, message, (message.From != null) ? message.Usr : message.From, message.PostedIn));
                        else
                        {
                            try
                            {
                                if (parameters[i].GetCustomAttribute<RemainderAttribute>() != null)
                                {
                                    if (paramType != typeof(string))
                                        throw new ArgumentException("Remaining only supports string.");
                                    args.Add(string.Join(" ", cmdParams.Skip(i)));
                                    break;
                                }
                                else args.Add(Convert.ChangeType(cmdParams[i], paramType));
                            }
                            catch (Exception)
                            {
                                Logger.Error("Error with argument conversion.");
                                // TODO: Fire OnError event
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
                        // TODO: OnCommandErrored
                    }
                }
            }
        }

        /// <summary>
        /// Starts the bot's primary loop. It takes a room name, and will attempt to join it.
        /// </summary>
        /// <param name="aRoomName">The name of the room you want to join</param>
        /// <returns>True if the bot started successfully, false otherwise.</returns>
        public async Task<bool> Connect(string aRoomName)
        {
            List<DrrrRoom> Rooms = await GetLounge();
            DrrrRoom Room = Rooms.Find(lRoom => lRoom.Name == aRoomName);
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
        /// <param name="aRoom">The room you want to join</param>
        /// <returns>True if the bot started successfully, false otherwise.</returns>
        public async Task<bool> Connect(DrrrRoom aRoom)
        {
            //If the need arises, processing can be done on the resulting DrrrRoom object.
            await JoinRoom(aRoom.RoomId);
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
        /// The bot's primary loop.
        /// </summary>
        private async Task BotMain()
        {
            Logger.Info("Starting loop.");

            while (Running)
            {
                DrrrRoom Data = await GetRoom();

                await Task.Delay(500);
            }
        }
    }
}
