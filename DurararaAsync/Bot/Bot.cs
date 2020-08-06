using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DrrrAsyncBot.Helpers;
using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Permission;

using Newtonsoft.Json;
using DrrrAsyncBot.BotExtensions;
using Newtonsoft.Json.Linq;

namespace DrrrAsyncBot.Core
{
    public partial class Bot : DrrrClient
    {
    
        private Dictionary<string, Command> Commands;
        public List<ICommandProcessor> commandProcessors;

        public DrrrBotConfig Config { get; private set; }

        private Bot(DrrrBotConfig config) : base (config.ProxyURI, config.ProxyPort)
        {
            Config = config;
            Commands = new Dictionary<string, Command>();
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

        public async void ReloadConfig(string FileName)
        {
            var json = "";
            using (var fs = File.OpenRead(FileName))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Config = JsonConvert.DeserializeObject<DrrrBotConfig>(json);

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

        public async new Task<bool> JoinRoom(string RoomName)
        {
            Logger.Debug("Retrieving roomlist.");
            var roomList = await GetLounge();

            Logger.Debug($"Searching for target room {RoomName}");
            DrrrRoom Found = roomList.Find(Room => Room.Name == RoomName);
            try
            {
                if (Found == null)
                {
                    if (Config.Room.Description != null)
                    {
                        Logger.Info("Creating room.");
                        await MakeRoom(Config.Room);
                        return true;
                    }
                    Logger.Info("Room does not exist.");
                }
                else if (Found.UserCount >= Found.Limit)
                    Logger.Warn("Room is full.");
                else if (Found.Users.Find(User => User.Name == Name) != null)
                    Logger.Error("User exists in room.");
                else
                {
                    Logger.Info("Joining room...");
                    await base.JoinRoom(Found.RoomId);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Fatal("Error encountered while joining room.", e);
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

        public async Task<bool> Reconnect(CancellationToken ShutdownToken)
        {    
            int Attempts = 0;
            Logger.Warn($"RECONNECT ROUTINE ACTIVE");
            await Task.Delay(2000);
            do
            {
                if(ShutdownToken.IsCancellationRequested)
                    return true;
                
                Logger.Info($"Attempting reconnect in 10 seconds. Attempt {++Attempts}");
                await Task.Delay(10000);
                JObject Profile = null;
                try
                {
                    Profile = await Get_Profile();
                }
                catch (Exception e)
                {
                    Logger.Error("Error getting profile.", e);
                    continue;
                }

                if (Profile.ContainsKey("message"))
                {
                    Logger.Debug($"User is logged in. Checking if room is valid.");

                    JObject Room;
                    try
                    { Room = await Get_Room_Raw(); }
                    catch (Exception e)
                    {
                        Logger.Error("Error getting room. Reconnect failed.", e);
                        return false;
                    }
                    if (Room.ContainsKey("message"))
                    {
                        Logger.Warn($"User is no longer in room.");
                        return false;
                    }

                }
                Logger.Done("Reconnect successful.");
                return true;
            }
            while (Attempts < 5);
            Logger.Fatal("Maximum reconnect attempts exceeded.");
            return false;
        }

        public async Task Setup()
        {
            Name = Config.Name;
            Logger.Info($"Logging in as : {Name}");
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

            Logger.Info("Update processor started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(500);
                if(cancellationToken.IsCancellationRequested)
                    break;
                await ProcessUpdate();
            }
            Logger.Info("Update processor exited.");
            await Task.Delay(500);
        }

        public async Task Resume(CancellationToken ShutdownToken)
        {
            Logger.Info("Update processor started.");
            while (!ShutdownToken.IsCancellationRequested)
            {
                await Task.Delay(500);
                if(ShutdownToken.IsCancellationRequested)
                    break;
                await ProcessUpdate();
            }
            Logger.Info("Update processor exited.");
            await Task.Delay(500);
        }

        /// <summary>
        /// MainAsync fire and forget. Creates it's own CancellationToken
        /// </summary>
        public async Task Run()
        {
            var Shutdown = new CancellationTokenSource();
            var ShutdownToken = Shutdown.Token;
            await MainAsync(ShutdownToken);
            Shutdown.Cancel();
        }
    }
}
