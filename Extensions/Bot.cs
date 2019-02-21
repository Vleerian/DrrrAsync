using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;
using DrrrAsync.Logging;
using DrrrAsync.Events;
using System.Linq;
using DrrrAsync.Extensions.Attributes;

namespace DrrrAsync.Extensions
{
    public class Bot : DrrrClient
    {
        // A dictionary of {"CommandName":CommandObject} used to invoke executed commands.
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

        public void Shutdown()
        {
            Running = false;
        }

        /// <summary>
        /// The register command goes through all the methods in command module, and those with the Command attribute
        /// are added to the CommandDictionary.
        /// </summary>
        /// <typeparam name="T">The CommandModule's type</typeparam>
        /// <param name="CommandModule">The command module.</param>
        public void RegisterCommands<T>() where T : class
        {
            dynamic Instance = Activator.CreateInstance<T>();
            foreach (MethodInfo Method in typeof(T).GetMethods())
            {
                // If it has the Command attribute, add it to the Command dictionary
                CommandAttribute CommandAttribute = Method.GetCustomAttribute<CommandAttribute>();
                if (CommandAttribute != null)
                {
                    DescriptionAttribute Desc = Method.GetCustomAttribute<DescriptionAttribute>();

                    Command CommandObject = new Command(Instance, Method, CommandAttribute.CommandName, Desc != null ? Desc.CommandDescription : "");

                    Commands.Add(CommandAttribute.CommandName, CommandObject);

                    //Get the aliases attribute, and add them to the Command dictionary
                    AliasesAttribute Aliases = Method.GetCustomAttribute<AliasesAttribute>();
                    Aliases?.AliasList.ForEach(Alias => Commands.Add(Alias, CommandObject));
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
            if (message.Mesg.StartsWith(CommandPrefix))
            {
                //Make sure a string is splittable before trying to split it
                string[] Parts;
                if (message.Mesg.Contains(" "))
                    Parts = message.Mesg.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                else
                    Parts = new string[] { message.Mesg };


                string Cmnd = Parts[0].ToLower().Substring(1);
                if (Commands.ContainsKey(Cmnd))
                {
                    // If a command or alias is in the CommandDictionary, execute it.
                    Context ctx = new Context(this, message, (message.From != null) ? message.Usr : message.From, message.PostedIn);

                    // Get parameter info from the command
                    ParameterInfo[] Inf = Commands[Cmnd].Method.GetParameters();

                    // Create the arg holder.
                    List<object> Args = new List<object>();

                    for (int i = 0; i < Inf.Length; i++)
                    {
                        // If the function calls for Context, give it to it.
                        Type ParamType = Inf[i].ParameterType;
                        if (ParamType == typeof(Context))
                            Args.Add(ctx);
                        else
                        {
                            //Add the arguments to the list
                            try
                            {
                                //If the parameter takes the remaining values, let it be
                                if (Inf[i].GetCustomAttribute<RemainingAttribute>() != null)
                                {
                                    //Remaining only supports string.
                                    if (ParamType != typeof(string))
                                        throw new ArgumentException("Remaining only supports string.");
                                    Args.Add(string.Join(" ", Parts.Skip(i)));
                                    break;
                                }
                                else
                                    Args.Add(Convert.ChangeType(Parts[i], ParamType));
                            }
                            catch (Exception)
                            {
                                Logger.Error("Error with argument conversion.");
                                //TODO: Fire OnError event
                            }
                        }
                    }

                    //Run the command and catch exceptions
                    try
                    {
                        await Commands[Cmnd].Call(Args.ToArray());
                    }
                    catch (Exception err)
                    {
                        Logger.Error($"Command [{Cmnd}] errored with message: {err.Message}");
                        //TODO: OnCommandErrored
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

            if (!Connected)
                return false;
            Console.WriteLine("Connecting...");

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
            Console.WriteLine("Done...");

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
