using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DrrrAsync
{
    class Client
    {
        //User Defined
        public string Name { get; private set; }
        public string Icon { get; private set; }

        //Site-Defined
        public string ID { get; private set; }
        public Objects.User User { get; private set; }

        //Client State
        public bool LoggedIn { get; private set; }

        //Client Extensions
        public CookieWebClient WC;
        //public EventHandler OnLogin;

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
            //OnLogin(this, new EventArgs());
            return true;
        }

        public async Task<Objects.Room[]> GetLounge()
        {
            Uri WebAddress = new Uri("http://drrr.com/lounge?api=json");
            JObject Lounge = JObject.Parse(await WC.DownloadStringTaskAsync(WebAddress));

            JObject Profile = Lounge.Value<JObject>("profile");
            ID = Profile["id"].Value<string>();
            Name = Profile["name"].Value<string>();
            User = new Objects.User(Profile);

            List<Objects.Room> Result = new List<Objects.Room>();
            foreach (JObject item in Lounge["rooms"])
            {
                Result.Add(new Objects.Room(item));
            }

            return Result.ToArray();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Client C = new Client("Welne Oren", "kuromu-2x");
            C.Login().GetAwaiter().GetResult();

            foreach (Objects.Room Room in C.GetLounge().GetAwaiter().GetResult())
            {
                Console.WriteLine($"{Room.Name} - {Room.Host.Name}");
            }

            Console.ReadKey();
        }
    }
}
