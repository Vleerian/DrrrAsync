using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrrrAsync.Objects
{
    [Serializable]
    public class DrrrRoomAPI
    {
        [JsonPropertyName("profile")]
        public DrrrUser profile { get; set; }
        [JsonPropertyName("user")]
        public DrrrUser user { get; set; }

        [JsonPropertyName("room")]
        public DrrrRoom room { get; set; }
    }

    /// <summary>
    /// A container for information pertaining to a room on Drrr.com
    /// </summary>
    [Serializable]
    public class DrrrRoom
    {
        public DrrrRoom()
        {
            Users = new List<DrrrUser>();
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
        [JsonPropertyName("update")]
        public double update { get; set; }

        [JsonPropertyName("users")]
        public List<DrrrUser> Users { get; set; }
        [JsonPropertyName("talks")]
        public List<DrrrMessage> Messages { get; set; }
        [JsonPropertyName("host")]
        public string host { get; set; }
        
        [JsonIgnore]
        public DrrrUser Host {
            get => Users.Where(U=>U.ID == host).FirstOrDefault();
        }

        public DrrrRoom Patch(DrrrRoom UpdateData)
        {
            if(UpdateData == null || UpdateData == default)
                return this;
            // These attributes are guaranteed with the room update
            Name = UpdateData.Name;
            Description = UpdateData.Description;
            limit = UpdateData.limit;

            // Update the user list if that is provided
            if(UpdateData.Users?.Count > 0)
                Users = UpdateData.Users;
            
            // Update the host
            if(UpdateData.Host != null)
                host = UpdateData.host;
            
            return this;
        }
    }
}