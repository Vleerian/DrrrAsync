using System;
using System.Collections.Generic;
using System.Text;

using DrrrBot.Objects;

namespace DrrrBot.Core
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
}
