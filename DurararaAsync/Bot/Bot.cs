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

using DrrrAsyncBot.Helpers;
using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Permission;

using Console = Colorful.Console;
using Newtonsoft.Json;
using DrrrAsyncBot.BotExtensions;
using Newtonsoft.Json.Linq;

namespace DrrrAsyncBot.Core
{
    public partial class Bot : DrrrClient
    {
        private Queue<DrrrMessageConfig> MessageQueue;
        private CancellationTokenSource ShutdownToken;

        private Dictionary<string, Command> Commands;
        public List<ICommandProcessor> commandProcessors;

        public DrrrBotConfig Config { get; private set; }

        private Bot(DrrrBotConfig config) : base (config.ProxyURI, config.ProxyPort)
        {
            Config = config;
            ShutdownToken = new CancellationTokenSource();
            Commands = new Dictionary<string, Command>();
            MessageQueue = new Queue<DrrrMessageConfig>();
            commandProcessors = new List<ICommandProcessor>() {
                new PermissionsProcessor()
            };

            //Set the default user agent to bot
            WebClient.DefaultRequestHeaders.Add("User-Agent", "Bot");
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

        private async void MessageLoop(CancellationToken cancellationToken)
        {
            await Task.Delay(500);
            DateTime LastSent = DateTime.Now;
            Logger.Log(LogEventType.Information, "Messageloop started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(250);
                if (cancellationToken.IsCancellationRequested)
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
                            Logger.Log(LogEventType.Error, "User is not in room.");
                            continue;
                        }
                    }

                    LastSent = DateTime.Now;
                    await base.SendMessage(Message.Message, Message.Url, Message.Username);
                }
                if ((DateTime.Now - LastSent).TotalMinutes >= 15)
                    await SendMessage("[HEARTBEAT]", Name);
            }
            Logger.Log(LogEventType.Information, "Messageloop exited.");
        }

        public async new Task<bool> JoinRoom(string RoomName)
        {
            Logger.Log(LogEventType.Debug, "Retrieving roomlist.");
            var roomList = await GetLounge();

            Logger.Log(LogEventType.Debug, "Searching for target room...");
            DrrrRoom Found = roomList.Find(Room => Room.Name == RoomName);
            try
            {
                if (Found == null)
                {
                    if (Config.Room.Description != null)
                    {
                        Logger.Log(LogEventType.Information, "Creating room.");
                        await MakeRoom(Config.Room);
                        return true;
                    }
                    Logger.Log(LogEventType.Information, "Room does not exist.");
                }
                else if (Found.UserCount >= Found.Limit)
                    Logger.Log(LogEventType.Information, "Room is full.");
                else if (Found.Users.Find(User => User.Name == Name) != null)
                    Logger.Log(LogEventType.Information, "User exists in room.");
                else
                {
                    Logger.Log(LogEventType.Information, "Joining room...");
                    await base.JoinRoom(Found.RoomId);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogEventType.Fatal, "Error encountered while joining room.", e);
            }
            return false;
        }

        public async Task ProcessUpdate()
        {
            foreach (var Message in await GetRoomUpdate())
            {
                if (Message.Type == DrrrMessageType.Message)
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

        public async Task<bool> Reconnect()
        {    
            int Attempts = 0;
            do
            {
                if(ShutdownToken.Token.IsCancellationRequested)
                    return true;
                
                Logger.Log(LogEventType.Information, $"Attempting reconnect in 10 seconds. Attempt {++Attempts}");
                await Task.Delay(10000);
                JObject Profile = null;
                try
                {
                    Profile = await Get_Profile();
                }
                catch (Exception e)
                {
                    Logger.Log(LogEventType.Error, "Error getting profile.", e);
                    continue;
                }

                if (Profile.ContainsKey("message"))
                {
                    Logger.Log(LogEventType.Debug, $"User is logged in. Checking if room is valid.");

                    JObject Room;
                    try
                    { Room = await Get_Room_Raw(); }
                    catch (Exception e)
                    {
                        Logger.Log(LogEventType.Error, "Error getting room. Reconnect failed.", e);
                        return false;
                    }
                    if (Room.ContainsKey("message"))
                    {
                        Logger.Log(LogEventType.Warning, $"User is no longer in room.");
                        return false;
                    }

                }
                Logger.Log(LogEventType.Information, "Reconnect successful.");
                return true;
            }
            while (Attempts < 5);
            Logger.Log(LogEventType.Fatal, "Maximum reconnect attempts exceeded.");
            return false;
        }

        public async Task Setup()
        {
            Name = Config.Name;
            Logger.Log(LogEventType.Information, "Logging in.");
            await Login();

            if(!await JoinRoom(Config.Room.Name))
                throw new ApplicationException("Unable to join room.");

            On_Post.Register(PrintMessage);
            On_Message.Register(ProcCommands);
            On_Direct_Message.Register(ProcCommands);

            //Print the room's history, if joining the room
            var JoinMessages = await GetRoom();
            foreach (var Message in JoinMessages.Messages)
                await PrintMessage(this, new AsyncMessageEvent(Message));
        }

        public async Task MainAsync(CancellationToken cancellationToken)
        {
            //Set up
            await Setup();
            
            MessageLoop(cancellationToken);

            Logger.Log(LogEventType.Information, "Update processor started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500);
                try
                {
                    await ProcessUpdate();
                }
                catch (Exception e)
                {
                    Logger.Log(LogEventType.Fatal, "Error encountered.", e);
                    if(await Reconnect()) continue;
                    Logger.Log(LogEventType.Fatal, "Reconnect failed.");
                }
            }

            await base.LeaveRoom();
        }

        public void Shutdown() =>
            ShutdownToken.Cancel();

        // Wraps your async main and provides services
        public void Run()
        {
            var cancellationToken = ShutdownToken.Token;

            //Launch the main function
            MainAsync(ShutdownToken.Token).GetAwaiter().GetResult();

            Shutdown();
        }
    }
}
