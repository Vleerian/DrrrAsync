using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a room on Drrr.com
    /// </summary>
    [Serializable]
    public class DrrrRoom
    {
        // Room properties
        [JsonProperty("language")]
        public string Language;
        [JsonProperty("roomId")]
        public string RoomId;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("limit")]
        public string limit { get; private set;}

        // Fix floating point room limits to avoid a potentially breaking change
        public int Limit { get {
            return int.Parse(limit.Split(".")[0]);
        }}
        public int UserCount { get => Users.Count; }
        public bool Full { get => Limit <= UserCount; }

        // Room flags
        [JsonProperty("staticRoom")]
        public bool StaticRoom;
        [JsonProperty("hiidenRoom")]
        public bool HiddenRoom;
        [JsonProperty("gameRoom")]
        public bool GameRoom;
        [JsonProperty("adultRoom")]
        public bool AdultRoom;
        [JsonProperty("music")]
        public bool Music;

        // Misc properties
        [JsonProperty("update")]
        public string Update;
        [JsonProperty("since")]
        public long opened;
        public DateTime Opened {
            get {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dtDateTime.AddSeconds(opened).ToLocalTime();
            }
        }

        [JsonProperty("users")]
        public List<DrrrUser> Users;
        [JsonProperty("talks")]
        public List<DrrrMessage> Messages;
        [JsonProperty("host")]
        public DrrrUser Host;

        public DrrrRoom()
        {
            Users = new List<DrrrUser>();
        }

        public DrrrRoom makerefs()
        {
            foreach (var message in Messages)
            {
                message.Room = this;
            }
            return this;
        }

        /// <summary>
        /// Attempts to get a user.
        /// </summary>
        /// <returns>True if the user was retrieved, false otherwise</returns>
        public bool TryGetUser(string Name, out DrrrUser RefUser)
        {
            RefUser = GetUser(Name);
            return RefUser != null;
        }

        /// <summary>
        /// Gets a user from a room
        /// </summary>
        /// <returns>The DrrrUser matching the provided name</returns>
        public DrrrUser GetUser(string Name)
        {
            return Users.Find(N => N.Name == Name);
        }

        /// <summary>
        /// Updates a room using data from a provided JObject containing room data.
        /// </summary>
        /// <param name="RoomObject">A JObject parsed using data from Drrr.com</param>
        /// <returns>A List<> of new DrrrMessages</returns>
        public List<DrrrMessage> UpdateRoom(DrrrRoom tmpRoom)
        {
            // Update the room's attributes
            Name = tmpRoom.Name;
            Description = tmpRoom.Description;
            limit = tmpRoom.limit;

            // Update the room's user list
            if(tmpRoom.Users.Count > 0)
                Users = tmpRoom.Users;

            // Update the host
            if(tmpRoom.Host != null)
                Host = tmpRoom.Host;

            // Update the room's message list, adding new messages
            // Populate a temporary list to return new messages only.
            List<DrrrMessage> New_Messages = new List<DrrrMessage>();
            if(tmpRoom.Messages.Count > 0)
            {
                foreach (DrrrMessage message in tmpRoom.Messages)
                {
                    // If it's not a room-profile message, and doesn't exist in the messages array
                    // Add it to New_Messages
                    if (message.type != "room-profile" &&
                        !Messages.Any(Mesg => Mesg.ID == message.ID))
                    {
                        Messages.Add(message);
                        New_Messages.Add(message);
                    }
                }
            }
            
            return New_Messages;
        }
    }
}