
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

using DrrrAsync.Objects;
using DrrrAsync.Helpers;
using DrrrAsync.Logging;

namespace DrrrAsync.Core
{
    public class DrrrClient
    {
        // User Defined
        public string Name;
        public DrrrIcon Icon;

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; protected set; }
        public DrrrRoom Room { get; protected set; }
        public DrrrLounge Lounge { get; protected set; }
        public DateTime StartedAt { get; private set; }
        // Client State
        public bool LoggedIn { get; private set; }

        // Client Components
        protected static CookieContainer clientCookies;
        protected static HttpClientHandler clientHandler;
        protected static HttpClient httpClient;

        public BasicLogger Logger { get; private set; }

        public DrrrClient()
        {
            Logger = new BasicLogger("DrrrClient", LogLevel.Information);

            clientCookies = new();
            clientHandler = new () { CookieContainer = clientCookies };
            httpClient = new(clientHandler);
        }

        /// <summary>
        /// Returns the Drrr-Session-1 cookie
        /// </summary>
        /// <returns></returns>
        public string GetClientToken() =>
            clientCookies.GetCookies(new Uri("https://drrr.com"))["drrr-session-1"].Value;

        /// <summary>
        /// Creates a session on Drrr.com
        /// </summary>
        /// <returns>True if success, throws an exception otherwise</returns>
        public async Task<bool> Login()
        {
            // Call the index API to get the token
            string token;
            {
                var index = await httpClient.GetAsync("https://drrr.com/?api=json");
                var json = await index.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                token = doc.RootElement.GetProperty("token").GetString();
            }

            // Send a second request to do the actual login.
            var response = await httpClient.PostAsync("https://drrr.com", new Dictionary<string, string>()
            {
                { "name", Name },
                { "icon", Icon.ID },
                { "token", token },
                { "login", "ENTER" },
                { "language", "en-US" }
            });

            return true;
        }

        /// <summary>
        /// Retrieves the Lounge on drrr.com
        /// </summary>
        /// <returns>A List of DrrrRoom objects</returns>
        public async Task<DrrrLounge> GetLounge()
        {
            // Retreive lounge's json
            Lounge = await httpClient.GetJsonAsync<DrrrLounge>("https://drrr.com/lounge?api=json");

            // Update the client's DrrrUser using the profile data provided
            User = Lounge.Profile;
            ID = User.ID;
            Name = User.Name;

            return Lounge;
        }

        public async Task<DrrrRoom> GetRoom()
        {
            var API = await httpClient.GetJsonAsync<DrrrRoomAPI>("https://drrr.com/room/?api=json");

            Room = API.room;
            User = API.user;

            return Room;
        }

        /// <summary>
        /// Gets the newest data only for the current room
        /// </summary>
        /// <returns>A List of DrrrMessage objects</returns>
        public async Task<DrrrRoom> GetRoomUpdate()
        {
            DrrrRoom tmpRoom = null;
            try{
                tmpRoom = await httpClient.GetJsonAsync<DrrrRoom>($"https://drrr.com/json.php?update={Room.update}");
            }
            catch (TaskCanceledException) { Logger.Warn("Timed out fetching room update data."); }
            catch (HttpRequestException) { Logger.Warn("502 error fetching room update data."); }
            
            tmpRoom.Messages.ForEach(M => M.Room = tmpRoom);
            
            return tmpRoom;
        }

        /// <summary>
        /// Joins a room on Drrr.com
        /// </summary>
        /// <param name="roomId">The ID of the room being joined</param>
        public async Task JoinRoom(string roomId)
        {
            // Join the room
            Logger.Info($"Joining room {roomId}");
            await httpClient.GetAsync($"http://drrr.com/room/?id={roomId}");
        }

        /// <summary>
        /// Creates a room on drrr.com
        /// </summary>
        /// <param name="Config">How the room will be configured</param>
        public async Task<string> MakeRoom(DrrrRoomConfig Config)
        {
            // Send a POST to create the room.
            var response = await httpClient.PostAsync("https://drrr.com/create_room/", new Dictionary<string, string>() {
                { "name", Config.Name },
                { "description", Config.Description },
                { "limit", Config.Limit.ToString() },
                { "language", Config.Language },
                { "adult", Config.Adult?"true":"false" },
                { "submit", "Create Room" },
                { "music", Config.Music?"true":"false" }
            });
            return response;
        }

        /// <summary>
        /// Leaves the current room
        /// </summary>
        /// <returns>Whatever the site returns</returns>
        public async Task<bool> LeaveRoom()
        {
            if ((DateTime.Now - StartedAt).TotalSeconds > 40)
            {
                await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                    { "leave", "leave" }
                });
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gives host to another user
        /// </summary>
        /// <param name="user">The user you want to give host</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> GiveHost(DrrrUser user)
        {
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "message", Message },
                { "url",     Url     },
                { "to",      To      }
            });
        }

        /// <summary>
        /// Gets the current user's profile
        /// </summary>
        /// <returns>A json object of the current user</returns>
        public async Task<DrrrUser> Get_Profile() =>
            await httpClient.GetJsonAsync<DrrrUser>("https://drrr.com/profile/?api=json");

        /// <summary>
        /// Sets the room's title
        /// </summary>
        /// <param name="Title">The new room title</param>
        /// <returns>Whatever the site returns</returns>
        public async Task<string> SetRoomTitle(string Title)
        {
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
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
            return await httpClient.PostAsync("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "music", "music" },
                { "name", Name },
                { "url", Url }
            });
        }
    }
}
