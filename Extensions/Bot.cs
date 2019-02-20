using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsync.Objects;
using DrrrAsync.Logging;
using DrrrAsync.Events;

namespace DrrrAsync
{
    namespace Extensions
    {
        public partial class Bot : DrrrClient
        {
            // A dictionary of {"CommandName":CommandObject} used to invoke executed commands.
            private Dictionary<string, Command> Commands;

            public bool Running { get; private set; }

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

            /// <summary>
            /// The register command goes through all the methods in command module, and those with the Command attribute
            /// are added to the CommandDictionary.
            /// </summary>
            /// <typeparam name="T">The CommandModule's type</typeparam>
            /// <param name="CommandModule">The command module.</param>
            public void RegisterCommands<T>() where T : class
            {
                // Get the command module's type, and iterate through it's methods
                Type ClassType = typeof(T);
                foreach (MethodInfo Method in ClassType.GetMethods())
                {
                    // If it has the Command attribute, add it to the Command dictionary
                    Attributes.CommandAttribute CommandAttribute = Method.GetCustomAttribute<Attributes.CommandAttribute>();
                    if(CommandAttribute != null)
                    {
                        Attributes.DescriptionAttribute Desc = Method.GetCustomAttribute<Attributes.DescriptionAttribute>();

                        Command CommandObject = new Command(Activator.CreateInstance<T>(), Method, CommandAttribute.CommandName, Desc!=null?Desc.CommandDescription:"");

                        Commands.Add(CommandAttribute.CommandName, CommandObject);

                        //Get the aliases attribute, and add them to the Command dictionary
                        Attributes.AliasesAttribute Aliases = Method.GetCustomAttribute<Attributes.AliasesAttribute>();
                        Aliases?.AliasList.ForEach(Alias => Commands.Add(Alias, CommandObject));
                    }
                }
            }

            /// <summary>
            /// The bot's command processor runs whenever a DrrrMessage event is thrown, and looks for commands.
            /// </summary>
            /// <param name="e">The DrrrMessage event.</param>
            private async Task LookForCommands(DrrrMessageEventArgs e)
            {
                DrrrMessage Message = e.DrrrMessage;
                // Check if the message starts with a command.
                // TODO: check for a command signal (I.E mesg.startswith(CommandSignal) where CommandSignal = '#')
                string Cmnd = Message.Mesg.Split(" ", 1, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
                if (Commands.ContainsKey(Cmnd))
                {
                    // If a command or alias is in the CommandDictionary, execute it.
                    Context ctx = new Context(this, Message, (Message.From != null) ? Message.Usr : Message.From, Message.PostedIn);
                    await Commands[Cmnd].Call(ctx);
                }

                await Task.CompletedTask;
            }

            /// <summary>
            /// The bot's primary loop. It takes a room name, and will attempt to join it.
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
                else
                    Connected = true;

                if (!Connected)
                    return false;
                Console.WriteLine("Connecting...");

                return await Connect(Room);
            }

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

            private async Task BotMain()
            {

                Console.WriteLine("Starting loop...");

                while (Running)
                {
                    if (Room != null)
                    {
                        DrrrRoom Data = await GetRoom();

                        await Task.Delay(500);
                    }
                }
            }
        }
    }
}
