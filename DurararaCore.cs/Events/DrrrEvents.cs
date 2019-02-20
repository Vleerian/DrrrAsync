using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync.Events
{
    public class DrrrEventArgs { }

    public class DrrrRoomEventArgs : DrrrEventArgs
    {
        public readonly DrrrRoom DrrrRoom;
        public DrrrRoomEventArgs(DrrrRoom room) => DrrrRoom = room;
        public static implicit operator DrrrRoomEventArgs(DrrrRoom room) => new DrrrRoomEventArgs(room);
    }

    public class DrrrUserEventArgs : DrrrEventArgs
    {
        public readonly DrrrUser DrrrUser;
        public DrrrUserEventArgs(DrrrUser user) => DrrrUser = user;
        public static implicit operator DrrrUserEventArgs(DrrrUser user) => new DrrrUserEventArgs(user);
    }
    public class DrrrMessageEventArgs : DrrrEventArgs
    {
        public readonly DrrrMessage DrrrMessage;
        public DrrrMessageEventArgs(DrrrMessage message) => DrrrMessage = message;
        public static implicit operator DrrrMessageEventArgs(DrrrMessage message) => new DrrrMessageEventArgs(message);
    }

    public delegate Task DrrrEventHandler();
    public delegate Task DrrrRoomEventHandler(DrrrRoomEventArgs _);
    public delegate Task DrrrUserEventHandler(DrrrUserEventArgs _);
    public delegate Task DrrrMessageEventHandler(DrrrMessageEventArgs _);
}