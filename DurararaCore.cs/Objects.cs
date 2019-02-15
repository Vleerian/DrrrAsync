using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DrrrAsync
{
    namespace Objects
    {
        public struct Room
        {
            public string Language { get; private set; }

            public string RoomId { get; private set; }

            public string Name { get; private set; }
            public string Description { get; private set; }
            public int Limit { get; private set; }
            public int UserCount {
                get {
                    return Users.Count;
                }
            }

            public DateTime Opened { get; private set; }

            public bool Music { get; private set; }
            public bool StaticRoom { get; private set; }
            public bool HiddenRoom { get; private set; }
            public bool GameRoom { get; private set; }
            public bool AdultRoom { get; private set; }
            public bool Full {
                get {
                    return Limit <= UserCount;
                }
            }

            public List<User> Users;
            public User Host;

            public Room(JObject RoomObject)
            {
                Language = RoomObject["language"].Value<string>();
                RoomId = RoomObject["id"].Value<string>();

                Name = RoomObject["name"].Value<string>();
                Description = RoomObject["description"].Value<string>();

                Limit = RoomObject["limit"].Value<int>();

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Opened = dtDateTime.AddSeconds(RoomObject["since"].Value<Int64>()).ToLocalTime();

                Music = RoomObject["music"].Value<bool>();
                StaticRoom = RoomObject["staticRoom"].Value<bool>();
                HiddenRoom = RoomObject["hiddenRoom"].Value<bool>();
                GameRoom = RoomObject["gameRoom"].Value<bool>();
                AdultRoom = RoomObject["adultRoom"].Value<bool>();

                Host = RoomObject.ContainsKey("host") ? new User((JObject)RoomObject["host"]) : new User();

                Users = new List<User>();
                foreach (JObject item in RoomObject["users"])
                {
                    Users.Add(new User(item));
                }
            }
        }

        public struct User
        {
            public string ID;
            public string Name;
            public string Tripcode;
            public string Icon;
            public string Device;
            public string LoggedIn;

            public User(JObject UserObject)
            {
                ID = UserObject.ContainsKey("id") ? UserObject["id"].Value<string>() : null;
                Name = UserObject["name"].Value<string>();
                Icon = UserObject.ContainsKey("icon") ? UserObject["icon"].Value<string>() : null;
                Tripcode = UserObject.ContainsKey("tripcode") ? UserObject["tripcode"].Value<string>() : null;
                Device = UserObject["device"].Value<string>();
                LoggedIn = UserObject.ContainsKey("loginedAt") ? UserObject["loginedAt"].Value<string>() : null;
            }
        }
    }
}
