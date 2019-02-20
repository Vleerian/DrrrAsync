using Newtonsoft.Json.Linq;

using DrrrAsync.AsyncEvents;

namespace DrrrAsync.Objects
{
    /// <summary>
    /// A container for information pertaining to a user on Drrr.Com
    /// </summary>
    public class DrrrUser : DrrrAsyncEventArgs
    {
        public string ID;
        public string Name;
        public string Tripcode;
        public string Icon;
        public string Device;
        public string LoggedIn;

        /// <summary>
        /// The DrrrUser constructor populates itself using a JObject parsed using data from Drrr.com.
        /// </summary>
        /// <param name="UserObject">A JOBject parsed using data from Drrr.com</param>
        public DrrrUser(JObject UserObject)
        {
            ID = UserObject.ContainsKey("id") ? UserObject["id"].Value<string>() : null;
            Name = UserObject["name"].Value<string>();
            Icon = UserObject.ContainsKey("icon") ? UserObject["icon"].Value<string>() : null;
            Tripcode = UserObject.ContainsKey("tripcode") ? UserObject["tripcode"].Value<string>() : null;
            Device = UserObject.ContainsKey("device") ? UserObject["device"].Value<string>() : null;
            LoggedIn = UserObject.ContainsKey("loginedAt") ? UserObject["loginedAt"].Value<string>() : null;
        }
    }
}
