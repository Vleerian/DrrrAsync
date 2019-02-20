using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using DrrrAsync.Objects;
using DrrrAsync.AsyncEvents;
using DrrrAsync.Logging;

namespace DrrrAsync
{
    public class DrrrClient
    {
        private readonly Logger Logger = new Logger { LogLevel = LogLevel.DEBUG, Name = "Client" };

        // User Defined
        private string name;
        public string Name {
            get => name;
            set
            {
                Logger.Name = $"Client \"{value}\"";
                name = value;
            }
        }
        public string Icon { get; private set; }

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }

        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        public CookieWebClient WebClient;

        public DrrrAsyncEvent OnLogin = new DrrrAsyncEvent();
        public DrrrAsyncEvent<DrrrRoom> OnRoomJoin = new DrrrAsyncEvent<DrrrRoom>();
        //public event EventHandler<DrrrUser> On_User_Joined  = new DrrrAsyncEvent<DrrrRoom>();
        public DrrrAsyncEvent<DrrrMessage> OnMessage = new DrrrAsyncEvent<DrrrMessage>();
        public DrrrAsyncEvent<DrrrMessage> OnDirectMessage = new DrrrAsyncEvent<DrrrMessage>();

        public DrrrClient(string aName, string aIcon)
        {
            WebClient = new CookieWebClient();

            foreach (var level in LogLevel.Levels)
                Logger.Log("DrrrClient Constructor", level);

            Name = aName;
            Icon = aIcon;
        }

        /// <summary>
        /// Logs the user in to Drrr.Com using the credentials set in the constructor.
        /// </summary>
        public async Task<bool> Login()
        {
            // Throw if Name or Icon are not set.
            if (Name == null)
                throw new ApplicationException("Name has not been set.");
            if (Icon == null)
                throw new ApplicationException("Icon has not been set.");

            Logger.Info($"Logging in with {Name}, {Icon}");

            // Get the index page to parse the token
            Uri WebAddress = new Uri("https://drrr.com");
            string IndexBody = await WebClient.DownloadStringTaskAsync(WebAddress);
            string Token = Regex.Match(IndexBody, @"""token"" data-value=""(\w+)\""").Groups[1].Value;

            // Send a second request to do the actual login.
            byte[] response = await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name",     Name    },
                { "icon",     Icon    },
                { "token",    Token   },
                { "login",    "ENTER" },
                { "language", "en-US" }
            });
            LoggedIn = true;
            OnLogin?.InvokeAsync();
            return true;
        }

        /// <summary>
        /// Fetches the lounge information from Drrr.com
        /// </summary>
        /// <returns>a List<> of DrrrRoom objects.</returns>
        public async Task<List<DrrrRoom>> GetLounge()
        {
            // Retreive the raw text of the lounge's json, and parse it
            Uri WebAddress = new Uri("http://drrr.com/lounge?api=json");
            JObject Lounge = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));

            // Update the client's DrrrUser using the profile data provided
            JObject Profile = Lounge.Value<JObject>("profile");
            ID = Profile["id"].Value<string>();
            Name = Profile["name"].Value<string>();
            User = new DrrrUser(Profile);

            // Return a List of visible rooms.
            List<DrrrRoom> Rooms = new List<DrrrRoom>();
            foreach (JObject item in Lounge["rooms"])
                Rooms.Add(new DrrrRoom(item));
            return Rooms;
        }

        /// <summary>
        /// Retrieves a room's updated data from Drrr.com
        /// </summary>
        /// <returns>A DrrrRoom object created with the update data.</returns>
        public async Task<DrrrRoom> GetRoom()
        {
            // Retrieve the room's data from the update api endpoint
            Uri WebAddress = new Uri($"http://drrr.com/json.php?update={Room.Update}");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));

            // Fire an event for each parsed message
            // Could this be moved to the DrrrRoom or DrrrMessage constructor in a clean way?
            foreach (DrrrMessage Mesg in Room.UpdateRoom(RoomData))
            {
                await OnMessage?.InvokeAsync(Mesg);
                // If it's a direct message, fire the OnDirectMessage event
                if (Mesg.Secret)
                    await OnDirectMessage?.InvokeAsync(Mesg);
            }

            return Room;
        }

        /// <summary>
        /// Joins a room on Drrr.com
        /// </summary>
        /// <param name="RoomId">The ID of the room, as found in GetLounge</param>
        /// <returns>A DrrrRoom object containing all the available data for that room.</returns>
        public async Task<DrrrRoom> JoinRoom(string RoomId)
        {
            // Join the room
            Logger.Info($"Joining room: {RoomId}");
            Uri WebAddress = new Uri($"http://drrr.com/room/?id={RoomId}");
            Logger.Debug($"URL: {WebAddress.AbsoluteUri}");
            await WebClient.DownloadStringTaskAsync(WebAddress);

            // Download and parse room data
            WebAddress = new Uri($"https://drrr.com/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);
            
            await OnRoomJoin?.InvokeAsync(Room);
            return Room;
        }

        /// <summary>
        /// Creates a room on Drrr.com
        /// </summary>
        /// <param name="aRoom">The DrrrRoom object with information set about the room you want to create.</param>
        /// <returns>All available data about the created room.</returns>
        public async Task<DrrrRoom> MakeRoom(DrrrRoom aRoom)
        {
            // Send a POST to create the room.
            Uri WebAddress = new Uri("https://drrr.com/create_room/?");
            byte[] response = await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name", aRoom.Name },
                { "description", aRoom.Description },
                { "limit", aRoom.Limit.ToString() },
                { "language", aRoom.Language },
                { "adult", aRoom.AdultRoom.ToString() },
            });

            // Retrieve room data
            WebAddress = new Uri($"http://drrr.com/room/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);
            
            await OnRoomJoin?.InvokeAsync(Room);
            return Room;
        }

        /// <summary>
        /// Leaves a room.
        /// NOTE: This method does not check whether or not you left successfully.
        /// If you have been in a room less than 30 seconds, you will recieve a 403 error.
        /// </summary>
        /// <returns>The raw byte[] data returned from the web request.</returns>
        public async Task<byte[]> LeaveRoom()
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "leave", "leave" }
            });
        }

        /// <summary>
        /// Gives host to another user.
        /// NOTE: This method does not check whether or not you have host, or if it was transferred succesffully.
        /// </summary>
        /// <param name="aUser">The user you want to give host to.</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> GiveHost(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "new_host", aUser.ID }
            });
        }

        /// <summary>
        /// Bans a user from the room
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully banned.
        /// </summary>
        /// <param name="aUser">The user you want to ban</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> Ban(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "ban", aUser.ID }
            });
        }

        /// <summary>
        /// Kicks a user from the room.
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully kicked.
        /// </summary>
        /// <param name="aUser">The user you want to kick</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> Kick(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "kick", aUser.ID }
            });
        }

        /// <summary>
        /// Sends a message to the room you are currently in.
        /// NOTE: This won't check whether or not you are in a room, and won't protect you from anti-spam measures.
        /// </summary>
        /// <param name="Message">The message you want to send</param>
        /// <param name="Url">The URL (if any) you want to attach.</param>
        /// <returns></returns>
        public async Task<byte[]> SendMessage(string Message, string Url = "")
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");

            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "message", Message },
                { "url",     Url     },
                { "to",      ""      }
            });
        }
    }
}
