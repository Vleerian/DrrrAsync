using System;
using System.Text.Json.Serialization;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a message on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrMessage
    {
        [JsonPropertyName("id")]
        public string ID;
        [JsonPropertyName("message")]
        public string Text;
        [JsonPropertyName("content")]
        public string Content;
        [JsonPropertyName("description")]
        public string Description;
        [JsonPropertyName("url")]
        public string Url;
        [JsonPropertyName("secret")]
        public bool Secret;

        [JsonPropertyName("time")]
        public double time;
        public DateTime Timestamp {
            get
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dtDateTime.AddSeconds(time).ToLocalTime();
            }
        }

        [JsonPropertyName("type")]
        public string type;
        public DrrrMessageType Type {
            get => (DrrrMessageType)type;
        }
        
        [JsonPropertyName("from")]
        public DrrrUser from;
        [JsonPropertyName("user")]
        public DrrrUser user;
        public DrrrUser Author
        {
            get => from ?? user;
        }
        
        [JsonPropertyName("to")]
        public DrrrUser Target;

        public DrrrRoom Room;
    }
}
