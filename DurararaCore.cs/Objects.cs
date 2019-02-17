using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DrrrAsync
{
    namespace Objects
    {
        public class DrrrRoom
        {
            public string Language { get; private set; }
            public string RoomId { get; private set; }
            public string Name { get; private set; }
            public string Description { get; private set; }
            public string Update { get; private set; }

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

            public List<DrrrUser> Users;
            public List<DrrrMessage> Messages;
            public DrrrUser Host;

            public DrrrRoom(JObject RoomObject)
            {
                Language = RoomObject["language"].Value<string>();
                RoomId = RoomObject["id"].Value<string>();
                Name = RoomObject["name"].Value<string>();
                Description = RoomObject["description"].Value<string>();
                Update = RoomObject["update"].Value<string>();

                Limit = RoomObject["limit"].Value<int>();

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Opened = dtDateTime.AddSeconds(RoomObject["since"].Value<Int64>()).ToLocalTime();

                Music = RoomObject["music"].Value<bool>();
                StaticRoom = RoomObject["staticRoom"].Value<bool>();
                HiddenRoom = RoomObject["hiddenRoom"].Value<bool>();
                GameRoom = RoomObject["gameRoom"].Value<bool>();
                AdultRoom = RoomObject["adultRoom"].Value<bool>();

                Users = new List<DrrrUser>();
                foreach (JObject item in RoomObject["users"])
                {
                    Users.Add(new DrrrUser(item));
                }

                Messages = new List<DrrrMessage>();
                foreach (JObject item in RoomObject["talks"])
                {
                    Messages.Add(new DrrrMessage(item, this));
                }

                Host = Users.Find(Usr => Usr.ID == RoomObject["host"].Value<string>());
            }

            public List<DrrrMessage> UpdateRoom(JObject RoomObject)
            {
                Name = RoomObject["name"].Value<string>();
                Description = RoomObject["description"].Value<string>();
                Limit = RoomObject["limit"].Value<int>();

                Users = new List<DrrrUser>();
                foreach (JObject item in RoomObject["users"])
                {
                    Users.Add(new DrrrUser(item));
                }

                List<DrrrMessage> New_Messages = new List<DrrrMessage>();
                foreach (JObject item in RoomObject["talks"])
                {
                    DrrrMessage tmp = new DrrrMessage(item, this);
                    if (!Messages.Any(Mesg=>Mesg.ID == tmp.ID))
                    {
                        Messages.Add(tmp);
                        New_Messages.Add(tmp);
                    }
                }

                Host = Users.Find(Usr => Usr.ID == RoomObject["host"].Value<string>());

                return New_Messages;
            }
        }

        public struct DrrrUser
        {
            public string ID;
            public string Name;
            public string Tripcode;
            public string Icon;
            public string Device;
            public string LoggedIn;

            public DrrrUser(JObject UserObject)
            {
                ID = UserObject.ContainsKey("id") ? UserObject["id"].Value<string>() : null;
                Name = UserObject["name"].Value<string>();
                Icon = UserObject.ContainsKey("icon") ? UserObject["icon"].Value<string>() : null;
                Tripcode = UserObject.ContainsKey("tripcode") ? UserObject["tripcode"].Value<string>() : null;
                Device = UserObject.ContainsKey("device") ? UserObject["device"].Value<string>() : null;
                LoggedIn = UserObject.ContainsKey("loginedAt") ? UserObject["loginedAt"].Value<string>() : null;
            }
        }

        public struct DrrrMessage
        {
            public string ID;
            public string Type;
            public string Mesg;
            public string Content;
            public string Url;

            public bool Secret;

            public DrrrRoom PostedIn;

            public DateTime Timestamp;
            public DrrrUser From;
            public DrrrUser To;
            public DrrrUser Usr;

            public DrrrMessage(JObject MessageObject, DrrrRoom aRoom)
            {
                ID = MessageObject["id"].Value<string>();
                Type = MessageObject["type"].Value<string>();
                Mesg = MessageObject["message"].Value<string>();
                Content = MessageObject.ContainsKey("content") ? MessageObject["content"].Value<string>() : null;
                Url = MessageObject.ContainsKey("url") ? MessageObject["url"].Value<string>() : null;

                Secret = MessageObject.ContainsKey("secret");

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                Timestamp = dtDateTime.AddSeconds(MessageObject["time"].Value<Int64>()).ToLocalTime();

                From = MessageObject.ContainsKey("from") ? new DrrrUser((JObject)MessageObject["from"]) : new DrrrUser();
                To = MessageObject.ContainsKey("to") ? new DrrrUser((JObject)MessageObject["to"]) : new DrrrUser();
                Usr = MessageObject.ContainsKey("user") ? new DrrrUser((JObject)MessageObject["user"]) : new DrrrUser();

                PostedIn = aRoom;
            }
        }
    }
}
