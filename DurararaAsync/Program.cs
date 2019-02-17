using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    class Client : IClient
    {
        //User Defined
        public string Name { get; private set; }
        public string Icon { get; private set; }

        //Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }

        //Client State
        public bool LoggedIn { get; private set; }

        //Client Extensions
        public CookieWebClient WC;

        public event EventHandler On_Login;
        public event EventHandler<DrrrRoom> On_Room_Join;
        public event EventHandler<DrrrUser> On_User_Joined;
        public event EventHandler<DrrrMessage> On_Message;
        public event EventHandler<DrrrMessage> On_Direct_Message;

        public Client(string aName, string aIcon)
        {
            WC = new CookieWebClient();

            Name = aName;
            Icon = aIcon;
        }

        public async Task<bool> Login()
        {
            //Validation. If the name and icon aren't set, throw errors.
            if (Name == null)
                throw new ApplicationException("Name has not been set.");
            if (Icon == null)
                throw new ApplicationException("Icon has not been set.");

            //Get the index page to parse the token
            Uri WebAddress = new Uri("https://drrr.com");
            string IndexBody = await WC.DownloadStringTaskAsync(WebAddress);
            string Token = Regex.Match(IndexBody, @"""token"" data-value=""(\w+)\""").Groups[1].Value;

            //Make a second request to do the actual login action.
            byte[] response = await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name",     Name    },
                { "icon",     Icon    },
                { "token",    Token   },
                { "login",    "ENTER" },
                { "language", "en-US" }
            });
            LoggedIn = true;
            On_Login(this, new EventArgs());
            return true;
        }

        public async Task<DrrrRoom[]> GetLounge()
        {
            Uri WebAddress = new Uri("http://drrr.com/lounge?api=json");
            JObject Lounge = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));

            JObject Profile = Lounge.Value<JObject>("profile");
            ID = Profile["id"].Value<string>();
            Name = Profile["name"].Value<string>();
            User = new DrrrUser(Profile);

            List<DrrrRoom> Result = new List<DrrrRoom>();
            foreach (JObject item in Lounge["rooms"])
            {
                Result.Add(new DrrrRoom(item));
            }

            return Result.ToArray();
        }

        public async Task<DrrrRoom> GetRoom()
        {
            Uri WebAddress = new Uri($"http://drrr.com/json.php?update={Room.Update}");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            foreach (DrrrMessage Mesg in Room.UpdateRoom(RoomData))
            {
                On_Message(this, Mesg);
                if (Mesg.Secret)
                    On_Direct_Message(this, Mesg);
            }

            return Room;
        }

        public async Task<DrrrRoom> JoinRoom(string RoomId)
        {
            //Join the room
            Uri WebAddress = new Uri($"http://drrr.com/room/?id={RoomId}");
            await WC.DownloadStringTaskAsync(WebAddress);

            //Download and parse the room's data
            WebAddress = new Uri($"http://drrr.com/room/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            //Run the On_Room_Join event, and return.
            On_Room_Join(this, Room);
            return Room;
        }

        public async Task<DrrrRoom> MakeRoom(DrrrRoom aRoom)
        {
            Uri WebAddress = new Uri("https://drrr.com/create_room/?");
            byte[] response = await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name",aRoom.Name },
                { "description",aRoom.Description },
                { "limit",aRoom.Limit.ToString()},
                { "language", aRoom.Language},
                { "adult", aRoom.AdultRoom.ToString()},
            });

            WebAddress = new Uri($"http://drrr.com/room/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            On_Room_Join(this, Room);
            return Room;
        }

        public async Task<byte[]> LeaveRoom(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            Room = null;
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "leave","leave"}
            });
        }

        public async Task<byte[]> GiveHost(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "new_host",aUser.ID}
            });
        }

        public async Task<byte[]> Ban(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "ban",aUser.ID}
            });
        }

        public async Task<byte[]> Kick(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "kick",aUser.ID}
            });
        }

        public async Task<byte[]> SendMessage(string Message, string Url = "")
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");

            return await WC.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "message",Message},
                { "url",    Url    },
                { "to",     ""     }
            });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Client C = new Client("Welne Oren", "kuromu-2x");
            C.Login().GetAwaiter().GetResult();

            foreach (DrrrRoom Room in C.GetLounge().GetAwaiter().GetResult())
            {
                Console.WriteLine($"{Room.Name} - {Room.Host.Name}");
            }

            Console.ReadKey();
        }
    }
}
