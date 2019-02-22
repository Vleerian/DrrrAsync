using System;
using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync.Events
{
    public delegate Task DrrrEventHandler(object Sender);
    public delegate Task DrrrEventError(object Sender, Exception _);
    public delegate Task DrrrRoomEventHandler(object Sender, DrrrRoom _);
    public delegate Task DrrrUserEventHandler(object Sender, DrrrUser _);
    public delegate Task DrrrMessageEventHandler(object Sender, DrrrMessage _);
}