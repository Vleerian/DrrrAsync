using System;
using System.Collections.Generic;
using System.Text;

namespace DrrrAsync.Objects
{
    public sealed class DrrrMessageType
    {
        public static readonly DrrrMessageType Message = new DrrrMessageType { ID = "message" };
        public static readonly DrrrMessageType Me = new DrrrMessageType { ID = "me" };
        public static readonly DrrrMessageType Roll = new DrrrMessageType { ID = "roll" };
        public static readonly DrrrMessageType Music = new DrrrMessageType { ID = "music" };
        public static readonly DrrrMessageType Kick = new DrrrMessageType { ID = "kick" };
        public static readonly DrrrMessageType Ban = new DrrrMessageType { ID = "ban" };
        public static readonly DrrrMessageType Join = new DrrrMessageType { ID = "join" };
        public static readonly DrrrMessageType Leave = new DrrrMessageType { ID = "leave" };
        public static readonly DrrrMessageType RoomProfile = new DrrrMessageType { ID = "room-profile" };

        public static Dictionary<string, DrrrMessageType> Types = new Dictionary<string, DrrrMessageType> {
            { "message", Message },
            { "me", Me },
            { "roll", Roll },
            { "music", Music },
            { "kick", Kick },
            { "ban", Ban },
            { "join", Join },
            { "leave", Leave },
            { "room-profile", RoomProfile }
        };

        public static implicit operator string(DrrrMessageType type) => type.ID;
        public static implicit operator DrrrMessageType(string type) => Types[type];

        public string ID { get; private set; }

        private DrrrMessageType() { }
    }
}
