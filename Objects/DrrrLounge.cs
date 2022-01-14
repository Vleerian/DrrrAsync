using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace DrrrAsync.Objects
{
    [Serializable]
    public class DrrrLounge
    {
        [JsonPropertyName("redirect")]
        public string redirect { get; set; }
        [JsonPropertyName("create_room_url")]
        public string create_room_url { get; set; }
        [JsonPropertyName("active_user")]
        public int Active_Users { get; set; }

        [JsonPropertyName("rooms")]
        public List<LoungeRoom> Rooms { get; set; }
        [JsonPropertyName("profile")]
        public DrrrUser Profile { get; set; }
    }
    
    [Serializable]
    public class LoungeRoom
    {
        public LoungeRoom()
        {
            Users = new();
        }

        // Room properties
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("roomId"), JsonConverter(typeof(AutoNumberToStringConverter))]
        public string RoomId { get; set; }
        [JsonPropertyName("name"), JsonConverter(typeof(AutoNumberToStringConverter))]
        public string Name { get; set; }
        [JsonPropertyName("description"), JsonConverter(typeof(AutoNumberToStringConverter))]
        public string Description { get; set; }

        [JsonPropertyName("limit")]
        public double limit { get; set;}

        // Fix floating point room limits to avoid a potentially breaking change
        [JsonIgnore]
        public int Limit { get {
            return (int)limit;
        }}
        [JsonIgnore]
        public int UserCount { get => Users.Count; }
        [JsonIgnore]
        public bool Full { get => Limit <= UserCount; }

        // Room flags
        [JsonPropertyName("staticRoom")]
        public bool StaticRoom { get; set; }
        [JsonPropertyName("hiidenRoom")]
        public bool HiddenRoom { get; set; }
        [JsonPropertyName("gameRoom")]
        public bool GameRoom { get; set; }
        [JsonPropertyName("adultRoom")]
        public bool AdultRoom { get; set; }
        [JsonPropertyName("music")]
        public bool Music { get; set; }

        [JsonPropertyName("since")]
        public long opened { get; set; }
        public DateTime Opened {
            get {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dtDateTime.AddSeconds(opened).ToLocalTime();
            }
        }

        [JsonPropertyName("users")]
        public List<DrrrUser> Users { get; set; }
        [JsonPropertyName("host")]
        public DrrrUser Host { get; set; }
    }
}