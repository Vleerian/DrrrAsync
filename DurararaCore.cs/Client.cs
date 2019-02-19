using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DrrrAsync.Objects;
using DrrrAsync.AsyncEvents;

namespace DrrrAsync
{
    public class DrrrClient
    {
        // User Defined
        public string Name { get; private set; }
        public string Icon { get; private set; }

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }

        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        public CookieWebClient WC;

        public DrrrAsyncEvent On_Login;
        public DrrrAsyncEvent<DrrrRoom> On_Room_Join;
        //public event EventHandler<DrrrUser> On_User_Joined;
        public DrrrAsyncEvent<DrrrMessage> On_Message;
        public DrrrAsyncEvent<DrrrMessage> On_Direct_Message;

        public DrrrClient(string aName, string aIcon)
        {
            WC = new CookieWebClient();

            Name = aName;
            Icon = aIcon;
        }

        public async Task<bool> Login()
        {
            Utility.Log('c', "Status", $"Logging in with {Name}, {Icon}", Utility.Log_Level.Status);
            // Validation. If the name and icon aren't set, throw errors.
            if (Name == null)
                throw new ApplicationException("Name has not been set.");
            if (Icon == null)
                throw new ApplicationException("Icon has not been set.");

            // Get the index page to parse the token
            Uri WebAddress = new Uri("https://drrr.com");
            string IndexBody = await WC.DownloadStringTaskAsync(WebAddress);
            string Token = Regex.Match(IndexBody, @"""token"" data-value=""(\w+)\""").Groups[1].Value;

            // Make a second request to do the actual login action.
            byte[] response = await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name",     Name    },
                { "icon",     Icon    },
                { "token",    Token   },
                { "login",    "ENTER" },
                { "language", "en-US" }
            });
            LoggedIn = true;
            On_Login?.InvokeAsync();
            return true;
        }

        public async Task<List<DrrrRoom>> GetLounge()
        {
            Uri WebAddress = new Uri("http://drrr.com/lounge?api=json");
            JObject Lounge = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));

            JObject Profile = Lounge.Value<JObject>("profile");
            ID = Profile["id"].Value<string>();
            Name = Profile["name"].Value<string>();
            User = new DrrrUser(Profile);

            List<DrrrRoom> Rooms = new List<DrrrRoom>();
            foreach (JObject item in Lounge["rooms"])
                Rooms.Add(new DrrrRoom(item));

            return Rooms;
        }

        public async Task<DrrrRoom> GetRoom()
        {
            Uri WebAddress = new Uri($"http://drrr.com/json.php?update={Room.Update}");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            foreach (DrrrMessage Mesg in Room.UpdateRoom(RoomData))
            {
                await On_Message?.InvokeAsync(Mesg);
                if (Mesg.Secret)
                    await On_Direct_Message?.InvokeAsync(Mesg);
            }

            return Room;
        }

        public async Task<DrrrRoom> JoinRoom(string RoomId)
        {
            Utility.Log('c', "Status", $"Joining room: {RoomId}", Utility.Log_Level.Status);
            // Join the room
            Uri WebAddress = new Uri($"http://drrr.com/room/?id={RoomId}");

            Utility.Log('c', "Status", $"URL: {WebAddress.AbsoluteUri}", Utility.Log_Level.Debug);

            await WC.DownloadStringTaskAsync(WebAddress);

            // Download and parse the room's data
            WebAddress = new Uri($"https://drrr.com/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            // Run the On_Room_Join event, and return.
            await On_Room_Join?.InvokeAsync(Room);
            return Room;
        }

        public async Task<DrrrRoom> MakeRoom(DrrrRoom aRoom)
        {
            Uri WebAddress = new Uri("https://drrr.com/create_room/?");
            byte[] response = await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name", aRoom.Name },
                { "description", aRoom.Description },
                { "limit", aRoom.Limit.ToString() },
                { "language", aRoom.Language },
                { "adult", aRoom.AdultRoom.ToString() },
            });

            WebAddress = new Uri($"http://drrr.com/room/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            await On_Room_Join?.InvokeAsync(Room);
            return Room;
        }

        public async Task<byte[]> LeaveRoom(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            Room = null;
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "leave", "leave" }
            });
        }

        public async Task<byte[]> GiveHost(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "new_host", aUser.ID }
            });
        }

        public async Task<byte[]> Ban(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "ban", aUser.ID }
            });
        }

        public async Task<byte[]> Kick(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "kick", aUser.ID }
            });
        }

        public async Task<byte[]> SendMessage(string Message, string Url = "")
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");

            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "message", Message },
                { "url",     Url     },
                { "to",      ""      }
            });
        }
    }
}
