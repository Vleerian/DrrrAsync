using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using DrrrAsync.Objects;
using DrrrAsync.Events;
using DrrrAsync.Logging;

namespace DrrrAsync
{
    public class DrrrClient
    {
        protected readonly Logger Logger = new Logger { LogLevel = LogLevel.DEBUG, Name = "Client \"DrrrBot\"" };

        // User Defined
        private string name = "DrrrBot";
        public string Name {
            get => name;
            set
            {
                name = value;
                Logger.Name = $"Client \"{name}\"";
            }
        }
        public DrrrIcon Icon { get; set; } = DrrrIcon.Kuromu2x;

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }

        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        public CookieWebClient WebClient = new CookieWebClient();

        // Client Events
        /*public DrrrAsyncEvent OnLogin = new DrrrAsyncEvent();
        public DrrrAsyncEvent<DrrrRoom> OnRoomJoin = new DrrrAsyncEvent<DrrrRoom>();
        //public event EventHandler<DrrrUser> On_User_Joined  = new DrrrAsyncEvent<DrrrRoom>();
        public DrrrAsyncEvent<DrrrMessage> OnMessage = new DrrrAsyncEvent<DrrrMessage>();
        public DrrrAsyncEvent<DrrrMessage> OnDirectMessage = new DrrrAsyncEvent<DrrrMessage>();/**/

#pragma warning disable CS1998
        public event DrrrEventHandler        OnLogin         = async () => { };
        public event DrrrRoomEventHandler    OnRoomJoin      = async (_) => { };
        public event DrrrUserEventHandler    OnUserJoined    = async (_) => { };
        public event DrrrMessageEventHandler OnMessage       = async (_) => { };
        public event DrrrMessageEventHandler OnDirectMessage = async (_) => { };
#pragma warning restore CS1998

        /// <summary>
        /// Logs the user in to Drrr.Com using the credentials set in the constructor.
        /// </summary>
        public async Task<bool> Login()
        {
            Logger.Info($"Logging in with {Name}, {Icon}");

            // Get the index page to parse the token
            string IndexBody = await WebClient.Get_String("https://drrr.com");
            string Token = Regex.Match(IndexBody, @"""token"" data-value=""(\w+)\""").Groups[1].Value;

            // Send a second request to do the actual login.
            string response = await WebClient.Post_String("https://drrr.com", new NameValueCollection() {
                { "name",     Name    },
                { "icon",     Icon.ID },
                { "token",    Token   },
                { "login",    "ENTER" },
                { "language", "en-US" }
            });

            LoggedIn = true;
#pragma warning disable CS4014
            OnLogin.FireEventAsync();
#pragma warning restore CS4014
            return true;
        }

        /// <summary>
        /// Fetches the lounge information from Drrr.com
        /// </summary>
        /// <returns>a List<> of DrrrRoom objects.</returns>
        public async Task<List<DrrrRoom>> GetLounge()
        {
            // Retreive lounge's json
            JObject Lounge = await WebClient.Get_Json("https://drrr.com/lounge?api=json");

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
            // Retrieve the room's data and fire an event for each parsed message
            foreach (DrrrMessage Mesg in Room.UpdateRoom(await WebClient.Get_Json($"https://drrr.com/json.php?update={Room.Update}")))
            {
                await OnMessage.FireEventAsync(Mesg);
                // If it's a direct message, fire the OnDirectMessage event
                if (Mesg.Secret)
                    await OnDirectMessage.FireEventAsync(Mesg);
            }
            return Room;
        }

        /// <summary>
        /// Joins a room on Drrr.com
        /// </summary>
        /// <param name="roomId">The ID of the room, as found in GetLounge</param>
        /// <returns>A DrrrRoom object containing all the available data for that room.</returns>
        public async Task<DrrrRoom> JoinRoom(string roomId)
        {
            // Join the room
            Logger.Info($"Joining room: {roomId}");
            Logger.Debug($"URL: http://drrr.com/room/?id={roomId}");

            await WebClient.Get_String($"http://drrr.com/room/?id={roomId}");

            // Download room data
            JObject RoomData = await WebClient.Get_Json($"https://drrr.com/json.php?fast=1");
            Room = new DrrrRoom(RoomData);
            
            await OnRoomJoin.FireEventAsync(Room);
            return Room;
        }

        /// <summary>
        /// Creates a room on Drrr.com
        /// </summary>
        /// <param name="room">The DrrrRoom object with information set about the room you want to create.</param>
        /// <returns>All available data about the created room.</returns>
        public async Task<DrrrRoom> MakeRoom(DrrrRoom room)
        {
            // Send a POST to create the room.
            string response = await WebClient.Post_String("https://drrr.com/create_room/?", new NameValueCollection() {
                { "name", room.Name },
                { "description", room.Description },
                { "limit", room.Limit.ToString() },
                { "language", room.Language },
                { "adult", room.AdultRoom.ToString() },
            });

            // Retrieve room data
            JObject RoomData = await WebClient.Get_Json("http://drrr.com/room/json.php?fast=1");
            Room = new DrrrRoom(RoomData);
            
            await OnRoomJoin.FireEventAsync(Room);
            return Room;
        }

        /// <summary>
        /// Leaves a room.
        /// NOTE: This method does not check whether or not you left successfully.
        /// If you have been in a room less than 30 seconds, you will recieve a 403 error.
        /// </summary>
        /// <returns>The raw byte[] data returned from the web request.</returns>
        public Task<string> LeaveRoom()
        {
            return WebClient.Post_String("https://drrr.com/room/?ajax=1", new NameValueCollection() {
                { "leave", "leave" }
            });
        }

        /// <summary>
        /// Gives host to another user.
        /// NOTE: This method does not check whether or not you have host, or if it was transferred succesffully.
        /// </summary>
        /// <param name="user">The user you want to give host to.</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public Task<string> GiveHost(DrrrUser user)
        {
            return WebClient.Post_String("https://drrr.com/room/?ajax=1", new NameValueCollection() {
                { "new_host", user.ID }
            });
        }

        /// <summary>
        /// Bans a user from the room
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully banned.
        /// </summary>
        /// <param name="user">The user you want to ban</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public Task<string> Ban(DrrrUser user)
        {
            return WebClient.Post_String("https://drrr.com/room/?ajax=1", new NameValueCollection() {
                { "ban", user.ID }
            });
        }

        /// <summary>
        /// Kicks a user from the room.
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully kicked.
        /// </summary>
        /// <param name="user">The user you want to kick</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public Task<string> Kick(DrrrUser user)
        {
            return WebClient.Post_String("https://drrr.com/room/?ajax=1", new NameValueCollection() {
                { "kick", user.ID }
            });
        }

        /// <summary>
        /// Sends a message to the room you are currently in.
        /// NOTE: This won't check whether or not you are in a room, and won't protect you from anti-spam measures.
        /// </summary>
        /// <param name="message">The message you want to send</param>
        /// <param name="url">The URL (if any) you want to attach.</param>
        /// <returns></returns>
        public Task<string> SendMessage(string message, string url = "")
        {
            return WebClient.Post_String("https://drrr.com/room/?ajax=1", new NameValueCollection() {
                { "message", message },
                { "url",     url     },
                { "to",      ""      }
            });
        }
    }
}
