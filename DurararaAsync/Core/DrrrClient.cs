using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DrrrBot.Objects;
using System.Drawing;

namespace DrrrBot.Core
{
    public class DrrrClient
    {
        // User Defined
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
            }
        }
        public DrrrIcon Icon { get; set; } = DrrrIcon.Kuromu2x;

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }
        public DateTime StartedAt { get; private set; }
        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        protected HttpClientE WebClient = new HttpClientE();

        // Events
        public DrrrAsyncEvent On_Login;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Join;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Message;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Direct_Message;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Kick;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Ban;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Leave;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Me;
        public DrrrAsyncEvent<AsyncMessageEvent> On_Post;

        public DrrrClient()
        {
            On_Login = new DrrrAsyncEvent();
            On_Join = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Direct_Message = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Kick = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Ban = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Leave = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Me = new DrrrAsyncEvent<AsyncMessageEvent>();
            On_Post = new DrrrAsyncEvent<AsyncMessageEvent>();
        }

        public async Task<bool> Login()
        {
            // Get the index page to parse the token
            string IndexBody = await WebClient.Get_String("https://drrr.com");
            string Token = Regex.Match(IndexBody, @"""token"" data-value=""(\w+)\""").Groups[1].Value;

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

        public async Task<DrrrRoom> GetRoom()
        {
            Room = new DrrrRoom(await WebClient.Get_Json($"https://drrr.com/json.php?fast=1"));
            return Room;
        }

        public async Task<List<DrrrMessage>> GetRoomUpdate() =>
            Room.UpdateRoom(await WebClient.Get_Json($"https://drrr.com/json.php?update={Room.Update}"));

        public async Task JoinRoom(string roomId)
        {
            // Join the room
            Helpers.Utilities.Log(Color.Cyan, "INFO", $"Joining room {roomId}");
            await WebClient.Get_String($"http://drrr.com/room/?id={roomId}");
        }

        public async Task MakeRoom(DrrrRoomConfig Config)
        {
            // Send a POST to create the room.
            string response = await WebClient.Post_String("https://drrr.com/create_room/", new Dictionary<string, string>() {
                { "name", Config.Name },
                { "description", Config.Description },
                { "limit", Config.Limit.ToString() },
                { "language", Config.Language },
                { "adult", Config.Adult.ToString() },
                { "submit", "Create Room" },
                { "music", Config.Music.ToString() }
            });
        }

        public async Task<string> LeaveRoom()
        {
            if ((DateTime.Now - StartedAt).TotalSeconds > 40)
                return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                    { "leave", "leave" }
                });
            return "Cannot Leave";
        }

        public async Task<string> GiveHost(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "new_host", user.ID }
            });
        }

        public async Task<string> Ban(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "ban", user.ID }
            });
        }

        public async Task<string> Report_And_Ban(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "report_and_ban_user", user.ID }
            });
        }

        public async Task<string> Kick(DrrrUser user)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "kick", user.ID }
            });
        }

        public virtual async Task<string> SendMessage(string Message, string Url = "", string To = "")
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "message", Message },
                { "url",     Url     },
                { "to",      To      }
            });
        }

        public async Task<string> SetRoomTitle(string Title)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "room_name", Title }
            });
        }

        public async Task<string> SetRoomDescription(string Description)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "room_description", Description }
            });
        }

        public async Task<string> PlayMusic(string Name, string Url)
        {
            return await WebClient.Post_String("https://drrr.com/room/?ajax=1", new Dictionary<string, string>() {
                { "music", "music" },
                { "name", "Name" },
                { "url", "Url" }
            });
        }
    }
}
