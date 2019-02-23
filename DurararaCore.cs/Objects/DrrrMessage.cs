using System;
using Newtonsoft.Json.Linq;

using DrrrAsync.Events;

namespace DrrrAsync.Objects
{
    /// <summary>
    /// A container for information pertaining to a message on Drrr.Com
    /// </summary>
    public class DrrrMessage
    {
        public string ID;
        public string Text;
        public string Content;
        public string Url;

        public bool Secret;

        public DrrrMessageType Type;
        public DrrrRoom PostedIn;
        public DateTime Timestamp;
        public DrrrUser From;
        public DrrrUser To;
        public DrrrUser Usr;

        /// <summary>
        /// The DrrrMessage constructor populates itself using a JObject parsed using data from Drrr.com.
        /// </summary>
        /// <param name="MessageObject">A JOBject parsed using data from Drrr.com</param>
        /// <param name="aRoom">The DrrrRoom object this message was posted in.</param>
        public DrrrMessage(JObject MessageObject, DrrrRoom aRoom)
        {
            // Set the message's properties
            ID = MessageObject["id"].Value<string>();
            Text = MessageObject["message"].Value<string>();
            Content = MessageObject.ContainsKey("content") ? MessageObject["content"].Value<string>() : null;
            Url = MessageObject.ContainsKey("url") ? MessageObject["url"].Value<string>() : null;

            Secret = MessageObject.ContainsKey("secret");

            PostedIn = aRoom;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Timestamp = dtDateTime.AddSeconds(MessageObject["time"].Value<Int64>()).ToLocalTime();

            Type = DrrrMessageType.Types[MessageObject["type"].Value<string>()];
            From = MessageObject.ContainsKey("from") ? new DrrrUser((JObject)MessageObject["from"]) : null;
            To = MessageObject.ContainsKey("to") ? new DrrrUser((JObject)MessageObject["to"]) : null;
            Usr = MessageObject.ContainsKey("user") ? new DrrrUser((JObject)MessageObject["user"]) : null;
        }
    }
}
