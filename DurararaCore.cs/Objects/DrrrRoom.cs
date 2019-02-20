using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using DrrrAsync.Events;

namespace DrrrAsync.Objects
{
    /// <summary>
    /// A container for information pertaining to a room on Drrr.com
    /// </summary>
    public class DrrrRoom
    {
        public string Language { get; private set; }
        public string RoomId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Update { get; private set; }
            
        public int Limit { get; private set; }
        public int UserCount { get => Users.Count; }
            
        public bool Music { get; private set; }
        public bool StaticRoom { get; private set; }
        public bool HiddenRoom { get; private set; }
        public bool GameRoom { get; private set; }
        public bool AdultRoom { get; private set; }
        public bool Full { get => Limit <= UserCount; }
            
        public DateTime Opened { get; private set; }
        public List<DrrrUser> Users;
        public List<DrrrMessage> Messages;
        public DrrrUser Host;

        /// <summary>
        /// The DrrrRoom constructor populates itself using a JObject parsed using data from Drrr.com.
        /// </summary>
        /// <param name="RoomObject">A JOBject parsed using data from Drrr.com</param>
        public DrrrRoom(JObject RoomObject)
        {
            // Parse all the string-type fields
            Language = RoomObject["language"].Value<string>();
            RoomId = RoomObject["roomId"].Value<string>();
            Name = RoomObject["name"].Value<string>();
            Description = RoomObject["description"].Value<string>();

            // The update field can only be retrived from the room, so set it to null if it cannot be retrieved
            Update = RoomObject.ContainsKey("update") ? RoomObject["update"].Value<string>() : null;

            // Parse the room's user limit
            Limit = RoomObject["limit"].Value<int>();

            // Set all the boolean values
            Music = RoomObject["music"].Value<bool>();
            StaticRoom = RoomObject["staticRoom"].Value<bool>();
            HiddenRoom = RoomObject["hiddenRoom"].Value<bool>();
            GameRoom = RoomObject["gameRoom"].Value<bool>();
            AdultRoom = RoomObject["adultRoom"].Value<bool>();

            // Parse the timestamp to a DateTime object
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Opened = dtDateTime.AddSeconds(RoomObject["since"].Value<Int64>()).ToLocalTime();

            // Populate the user list.
            Users = new List<DrrrUser>();
            foreach (JObject item in RoomObject["users"])
                Users.Add(new DrrrUser(item));

            // The 'host' field is different depending on if data is retrived from the lounge or room endpoints.
            if (RoomObject["host"].Type == JTokenType.Object)
                Host = new DrrrUser((JObject)RoomObject["host"]);
            else
                Host = Users.Find(Usr => Usr.ID == RoomObject["host"].Value<string>());

            // Parse messages.
            Messages = new List<DrrrMessage>();
            if (RoomObject.ContainsKey("talks"))
                foreach (JObject item in RoomObject["talks"])
                    Messages.Add(new DrrrMessage(item, this));
        }

        /// <summary>
        /// Updates a room using data from a provided JObject containing room data.
        /// </summary>
        /// <param name="RoomObject">A JObject parsed using data from Drrr.com</param>
        /// <returns>A List<> of new DrrrMessages</returns>
        public List<DrrrMessage> UpdateRoom(JObject RoomObject)
        {
            // Update the room's attributes
            Name = RoomObject["name"].Value<string>();
            Description = RoomObject["description"].Value<string>();
            Limit = RoomObject["limit"].Value<int>();

            // Update the room's user list
            if(RoomObject.ContainsKey("users"))
            {
                Users = new List<DrrrUser>();
                foreach (JObject item in RoomObject["users"])
                    Users.Add(new DrrrUser(item));
            }

            // Update the host
            if(RoomObject.ContainsKey("host"))
                Host = Users.Find(Usr => Usr.ID == RoomObject["host"].Value<string>());

            // Update the room's message list, adding new messages
            // Populate a temporary list to return new messages only.
            List<DrrrMessage> New_Messages = new List<DrrrMessage>();
            if(RoomObject.ContainsKey("talks"))
            {
                foreach (JObject item in RoomObject["talks"])
                {
                    DrrrMessage tmp = new DrrrMessage(item, this);
                    if (!Messages.Any(Mesg => Mesg.ID == tmp.ID))
                    {
                        Messages.Add(tmp);
                        New_Messages.Add(tmp);
                    }
                }
            }
            return New_Messages;
        }
    }
}
