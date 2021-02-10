using System;
using System.Collections.Generic;
using System.Text;

using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Logging;

namespace DrrrAsyncBot.Core
{
    public class DrrrAsyncEventArgs
    {
        public bool Handled { get; set; }
    }

    public class AsyncMessageEvent : DrrrAsyncEventArgs
    {
        public DrrrMessage Message;

        public AsyncMessageEvent(DrrrMessage aMessage) =>
            Message = aMessage;
    }

    public class AsyncUserEvent : DrrrAsyncEventArgs
    {
        public DrrrUser User;

        public AsyncUserEvent(DrrrUser aUser) =>
            User = aUser;
    }

    public class AsyncRoomEvent : DrrrAsyncEventArgs
    {
        public DrrrRoom Room;

        public AsyncRoomEvent(DrrrRoom aRoom) =>
            Room = aRoom;
    }

    public class AsyncUserUpdateEventArgs : DrrrAsyncEventArgs
    {
        public DrrrRoom Room;
        public DrrrUser User;
        public LogLevel type;

        public AsyncUserUpdateEventArgs(DrrrRoom room, DrrrUser user)
        {
            Room = room;
            User = user;
        }
    }

    public class AsyncRoomUpdateEventArgs : DrrrAsyncEventArgs
    {
        public readonly DrrrRoom OldRoom;
        public readonly DrrrRoom NewRoom;
        public LogLevel type;

        public AsyncRoomUpdateEventArgs(DrrrRoom oldRoom, DrrrRoom newRoom)
        {
            OldRoom = oldRoom;
            NewRoom = newRoom;
        }
    }
}
