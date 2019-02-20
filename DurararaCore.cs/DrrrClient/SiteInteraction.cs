using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    public partial class DrrrClient
    {
        /// <summary>
        /// Logs the user in to Drrr.Com using the credentials set in the constructor.
        /// </summary>
        public async Task<bool> Login()
        {
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
    }
}
