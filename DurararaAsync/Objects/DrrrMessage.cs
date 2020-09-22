using System;

using Newtonsoft.Json;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a message on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrMessage
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("message")]
        public string Text;
        [JsonProperty("content")]
        public string Content;
        [JsonProperty("url")]
        public string Url;
        [JsonProperty("secret")]
        public bool Secret;

        [JsonProperty("time")]
        public double time;
        public DateTime Timestamp {
            get
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dtDateTime.AddSeconds(time).ToLocalTime();
            }
        }

        [JsonProperty("type")]
        public string type;
        public DrrrMessageType Type {
            get => (DrrrMessageType)type;
        }
        
        [JsonProperty("from")]
        public DrrrUser from;
        [JsonProperty("user")]
        public DrrrUser user;
        public DrrrUser Author
        {
            get => from ?? user;
        }
        
        [JsonProperty("to")]
        public DrrrUser Target;

        public DrrrRoom Room;
    }
}
