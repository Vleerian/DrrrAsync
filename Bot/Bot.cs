using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using DrrrAsync.Helpers;
using DrrrAsync.Objects;

namespace DrrrAsync.Core
{
    public enum PermLevel {
        None = 0,
        Trusted = 1,
        Moderator = 2,
        Admin = 3,
        Operator = 4,
        Owner = 5
    }

    public partial class Bot : DrrrClient
    {
        private Dictionary<string, Command> Commands;

        public DrrrBotConfig Config { get; private set; }
        private string ConfigFile;
        
        // Delegatges
        public delegate void MessageEvent(Bot sender, AsyncMessageEvent args);

        // Events
        public event MessageEvent OnMessage;
        public event MessageEvent OnDirectMessage;
        public event MessageEvent OnJoin;
        public event MessageEvent OnLeave;
        public event MessageEvent OnPost;
        
        // Controls the poll speed.
        private int pollSpeed;
        public int PollSpeed {
            get { return pollSpeed; }
            set {
                if ( value < 500 ) pollSpeed = 500;
                else pollSpeed = value;
            }
        }

        private Bot(DrrrBotConfig config, string configFile)
        {
            pollSpeed = 500;
            Config = config;
            ConfigFile = configFile;
            Commands = new Dictionary<string, Command>();

            base.Name = config.Name;
            base.Icon = config.Icon;

            //Set the default user agent to bot
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Bot");
        }

        /// <summary>
        /// Uses a DrrrBotConfig object to create a bot.
        /// </summary>
        /// <param name="Config">See: DrrrBotConfig</param>
        /// <returns>An instance of Bot</returns>
        public static Bot Create(string ConfigFile)
        {
            var json = "";
            using (var fs = File.OpenRead(ConfigFile))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();
            var Config = JsonSerializer.Deserialize<DrrrBotConfig>(json);

            if (Config.Name == null || Config.Name == "")
                throw new Exception("No Name.");
            else if (Config.Room.Name == null || Config.Room.Name == "")
                throw new Exception("No Room Name.");
            else if (Config.CommandSignal == null || Config.CommandSignal == "")
                throw new Exception("No Command Signal.");
            else if (Config.Icon == null)
                throw new Exception("No Icon.");

            return new Bot(Config, ConfigFile);
        }

        public async void ReloadConfig()
        {
            var json = "";
            using (var fs = File.OpenRead(ConfigFile))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();
            Config = JsonSerializer.Deserialize<DrrrBotConfig>(json);

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

        public async new Task<bool> JoinRoom(string RoomName)
        {
            Logger.Debug($"{Name} - Retrieving roomlist.");
            var roomList = await GetLounge();

            Logger.Debug($"{Name} - Searching for target room {RoomName}");
            LoungeRoom Found = roomList.Rooms.Where(Room => Room.Name == RoomName).FirstOrDefault();
            try
            {
                if (Found == null)
                {
                    if (Config.Room.Description != null)
                    {
                        Logger.Info($"{Name} - Creating room.");
                        await MakeRoom(Config.Room);
                        return true;
                    }
                    Logger.Info($"{Name} - Room does not exist.");
                }
                else if (Found.UserCount >= Found.Limit)
                    Logger.Warn($"{Name} - Room is full.");
                else if (Found.Users.Find(User => User.Name == Name) != null)
                    Logger.Error($"{Name} - User exists in room.");
                else
                {
                    Logger.Info($"{Name} - Joining room...");
                    await base.JoinRoom(Found.RoomId);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Fatal($"{Name} - Error encountered while joining room.", e);
            }
            return false;
        }

        double LastTime = 0;
        public async void WorkLoop(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await Task.Delay(PollSpeed);

                IEnumerable<DrrrMessage> Messages;
                try{
                    var Room = await GetRoomUpdate();
                    if(Room == null || Room.Messages == null)
                        continue;
                    Messages = Room.Messages.Where(M => M.time > LastTime);
                }
                catch(Exception e)
                {
                    Logger.Error("Error in work loop", e);
                    continue;
                }

                foreach (var Message in Messages)
                {
                    switch(Message.Type.ID)
                    {
                        case "message":
                        case "me":
                            if (Message.Secret)
                                OnDirectMessage?.Invoke(this, new AsyncMessageEvent(Message));
                            else
                                OnMessage?.Invoke(this, new AsyncMessageEvent(Message));
                            break;
                        case "join":
                            OnJoin?.Invoke(this, new AsyncMessageEvent(Message));
                            break;
                        case "leave":
                            OnLeave?.Invoke(this, new AsyncMessageEvent(Message));
                            break;
                    }
                    OnPost?.Invoke(this, new AsyncMessageEvent(Message));
                }
                var last = Messages.FirstOrDefault();
                if(last != default)
                    LastTime = last.time;
            }
        }

        public async void MainAsync(CancellationToken cancellationToken)
        {
            // Log in to the site
            await Login();

            // Set up events
            OnMessage += ProcCommands;
            OnDirectMessage += ProcCommands;
            OnPost += PrintMessage;

            // Join the target room
            await JoinRoom(Config.Room.Name);
            // Print message history, set the LastTime variable, and define user state
            await GetRoom();
            Room.Messages.ForEach(M => PrintMessage(this, new AsyncMessageEvent(M)));
            LastTime = Room.Messages.FirstOrDefault().time;

            // start the work loop
            WorkLoop(cancellationToken);

            // We set the HeartBeat timer to 20 minutes before start. See README
            DateTime HeartBeat = DateTime.Now.AddMinutes(-20);

            Logger.Info($"{Name} - Update processor started.");
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(pollSpeed);
                if(cancellationToken.IsCancellationRequested)
                    break;

                // We process the heartbeat before update. See README
                var diff = (DateTime.Now - HeartBeat).TotalMinutes;
                if(diff >= 10)
                {
                    await SendMessage("[HEARTBEAT]", To:User.ID);
                    HeartBeat = DateTime.Now;
                }
            }
            Logger.Info($"{Name} - Update processor exited.");
            await Task.Delay(pollSpeed);
        }

        /// <summary>
        /// MainAsync fire and forget. Creates it's own CancellationToken
        /// </summary>
        public CancellationTokenSource Run()
        {
            var Shutdown = new CancellationTokenSource();
            var ShutdownToken = Shutdown.Token;
            MainAsync(ShutdownToken);
            return Shutdown;
        }
    }
}
