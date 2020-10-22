
using System.Net;
using System.Security.AccessControl;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Helpers;

using Newtonsoft.Json.Linq;

namespace DrrrAsyncBot.Core
{
    public class DrrrClient
    {
        // User Defined
        public string Name;
        public DrrrIcon Icon;

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }
        public DateTime StartedAt { get; private set; }
        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        protected HttpClientE WebClient;

        // Events
        public DrrrAsyncEvent On_Login;
        public DrrrAsyncEvent On_Update;
        public DrrrAsyncEvent On_Ready;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Join;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Message;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Direct_Message;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Kick;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Ban;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Leave;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Me;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Post;

        public DrrrClient(string ProxyURI = null, int ProxyPort = 0)
        {
            On_Login = new DrrrAsyncEvent();
            On_Update = new DrrrAsyncEvent();
            On_Ready = new DrrrAsyncEvent();
            On_Join = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Direct_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Kick = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Ban = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Leave = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Me = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Post = new DrrrAsyncEvent<AsyncMessageEvent>();

            WebClient = (ProxyURI != null) ? HttpClientE.GetProxyClient(ProxyURI, ProxyPort) : new HttpClientE();
            WebClient.Timeout = new TimeSpan(0, 0, 10);
            WebClient.DefaultRequestHeaders.Add("User-Agent", "Bot");
        }

        public DrrrClient()
        {
            On_Login = new DrrrAsyncEvent();
            On_Update = new DrrrAsyncEvent();
            On_Ready = new DrrrAsyncEvent();
            On_Join = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Direct_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Kick = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Ban = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Leave = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Me = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Post = new DrrrAsyncEvent<AsyncMessageEvent>();

            WebClient = new HttpClientE();
            WebClient.Timeout = new TimeSpan(0, 0, 10);
            WebClient.DefaultRequestHeaders.Add("User-Agent", "Bot");

        }

        /// <summary>
        /// Creates a session on Drrr.com
        /// </summary>
        /// <returns>True if success, throws an exception otherwise</returns>
        public async Task<bool> Login()
        {
            // Call the index API to get the token
            JObject APICall = await WebClient.Get_Json("https://drrr.com/?api=json");
            string Token = APICall.Value<string>("token");

            // Send a second request to do the actual login.
            string response = await WebClient.Post_String("https://drrr.com", new Dictionary<string, string>() {
                { "name",     Name    },
                { "icon",     Icon.ID },
                { "token",    Token   },
                { "login",    "ENTER" },
                { "language", "en-US" }
            });

            return true;
        }

        /// <summary>
        /// Retrieves the Lounge on drrr.com
        /// </summary>
        /// <returns>A List of DrrrRoom objects</returns>
        public async Task<List<DrrrRoom>> GetLounge()
        {
            // Retreive lounge's json
            DrrrLounge Lounge = await WebClient.Get_Object<DrrrLounge>("https://drrr.com/lounge?api=json");

            // Update the client's DrrrUser using the profile data provided
            User = Lounge.Profile;
            ID = User.ID;
            Name = User.Name;

            // Build the references for each room
            Lounge.Rooms.Select(Room=>Room.makerefs());
            // Return a List of visible rooms.
            return Lounge.Rooms;
        }

        /// <summary>
        /// Gets data from the current room on Drrr.com
        /// </summary>
        /// <returns>A DrrrRoom object</returns>
        public async Task<DrrrRoom> GetRoom()
        {
            Room = await WebClient.Get_Object<DrrrRoomFast>("https://drrr.com/json.php?fast=1");
            Room.makerefs();
            return Room;
        }

        /// <summary>
        /// Gets the newest data only for the current room
        /// </summary>
        /// <returns>A List of DrrrMessage objects</returns>
        public async Task<List<DrrrMessage>> GetRoomUpdate()
        {
            DrrrRoom tmpRoom = await WebClient.Get_Object<DrrrRoom>($"https://drrr.com/json.php?update={Room.Update}");
            if(tmpRoom == null)
                return new List<DrrrMessage>();
            Room.makerefs();
            return Room.UpdateRoom(tmpRoom);
        }

        /// <summary>
        /// Joins a room on Drrr.com
        /// </summary>
        /// <param name="roomId">The ID of the room being joined</param>
        public async Task JoinRoom(string roomId)
        {
            // Join the room
            Logger.Info($"Joining room {roomId}");
            await WebClient.Get_String($"http://drrr.com/room/?id={roomId}");
        }

        /// <summary>
        /// Creates a room on drrr.com
        /// </summary>
        /// <param name="Config">How the room will be configured</param>
        public async Task MakeRoom(DrrrRoomConfig Config)
        {
            // Send a POST to create the room.
            string response = await WebClient.Post_String("https://drrr.com/create_room/", new Dictionary<string, string>() {
                { "name", Config.Name },
                { "description", Config.Description },
                { "limit", Config.Limit.ToString() },
                { "language", Config.Language },
                { "adult", Config.Adult?"true":"false" },
                { "submit", "Create Room" },
                { "music", Config.Music?"true":"false" }
            });
        }

        /// <summary>
        /// Leaves the current room
        /// </summary>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> LeaveRoom()
        {
            if ((DateTime.Now - StartedAt).TotalSeconds > 40)
                return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                    { "leave", "leave" }
                });
            return "Cannot Leave";
        }

        /// <summary>
        /// Gives host to another user
        /// </summary>
        /// <param name="user">The user you want to give host</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> GiveHost(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "new_host", user.ID }
            });
        }

        /// <summary>
        /// Bans a user (NOT PERMANENT, CAN BE EASILY CIRCUMVENTED)
        /// </summary>
        /// <param name="user">The user you want to ban</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> Ban(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "ban", user.ID }
            });
        }

        /// <summary>
        /// Reports a bans a user (IP Ban)
        /// </summary>
        /// <param name="user">The user you want to ban</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> Report_And_Ban(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "report_and_ban_user", user.ID }
            });
        }

        /// <summary>
        /// Kicks a user from the room
        /// </summary>
        /// <param name="user">The user you want to kick</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> Kick(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "kick", user.ID }
            });
        }

        /// <summary>
        /// Sends a message to the room
        /// </summary>
        /// <param name="Message">The message being sent</param>
        /// <param name="Url">Optional: URL to attach</param>
        /// <param name="To">Optional: UserID of a user you want to DM</param>
        /// <returns>Whatever the site returns</returns>
        public virtual async Task<string> SendMessage(string Message, string Url = "", string To = "")
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "message", Message },
                { "url",     Url     },
                { "to",      To      }
            });
        }

        /// <summary>
        /// Gets the current user's profile
        /// </summary>
        /// <returns>A json object of the current user</returns>
        public async Task<JObject> Get_Profile() =>
            await WebClient.Get_Json("https://drrr.com/profile/?api=json");

        /// <summary>
        /// Gets the raw JOBject for the current room
        /// </summary>
        /// <returns>The raw room JOBject</returns>
        public async Task<JObject> Get_Room_Raw() =>
            await WebClient.Get_Json("https://drrr.com/room/?api=json");

        /// <summary>
        /// Sets the room's title
        /// </summary>
        /// <param name="Title">The new room title</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> SetRoomTitle(string Title)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "room_name", Title }
            });
        }

        /// <summary>
        /// Sets the room's description
        /// </summary>
        /// <param name="Description">The new description</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> SetRoomDescription(string Description)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "room_description", Description }
            });
        }

        /// <summary>
        /// Plays music in a room
        /// </summary>
        /// <param name="Name">The 'song name' displayed by the site</param>
        /// <param name="Url">The URL of the song</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> PlayMusic(string Name, string Url)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "music", "music" },
                { "name", Name },
                { "url", Url }
            });
        }
    }
}
