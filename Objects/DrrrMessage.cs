using System;
using System.Text.Json.Serialization;

namespace DrrrAsync.Objects
{
    /// <summary>
    /// A container for information pertaining to a message on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrMessage
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("message")]
        public string Text { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("secret")]
        public bool Secret { get; set; }

        [JsonPropertyName("time")]
        public double time { get; set; }

        [JsonIgnore]
        public DateTime Timestamp {
            get
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dtDateTime.AddSeconds(time).ToLocalTime();
            }
        }

        [JsonPropertyName("type")]
        public string type { get; set; }
        [JsonIgnore]
        public DrrrMessageType Type {
            get => (DrrrMessageType)type;
        }
        
        [JsonPropertyName("from")]
        public DrrrUser from { get; set; }
        [JsonPropertyName("user")]
        public DrrrUser user { get; set; }
        [JsonIgnore]
        public DrrrUser Author
        {
            get => from ?? user;
        }
        
        [JsonPropertyName("to")]
        public DrrrUser Target { get; set; }
        [JsonIgnore]
        public DrrrRoom Room { get; set; }
    }
}
