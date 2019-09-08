using System;
using Newtonsoft.Json.Linq;

namespace DrrrBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a message on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrMessage
    {
        public string ID;
        public string Text;
        public string Content;
        public string Url;

        public bool Secret;

        public DrrrMessageType Type;
        public DrrrRoom Room;
        public DateTime Timestamp;
        public DrrrUser Author;
        public DrrrUser Target;

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

            Room = aRoom;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Timestamp = dtDateTime.AddSeconds(MessageObject["time"].Value<Int64>()).ToLocalTime();

            Type = DrrrMessageType.Types[MessageObject["type"].Value<string>()];

            Author = null;
            if(MessageObject.ContainsKey("from") || MessageObject.ContainsKey("user"))
                Author = new DrrrUser((JObject)MessageObject["from"] ?? (JObject)MessageObject["user"]);

            Target = MessageObject.ContainsKey("to") ? new DrrrUser((JObject)MessageObject["to"]) : null;
        }

        protected DrrrMessage() { }
    }
}
