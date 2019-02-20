using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync.Events
{
    public delegate Task DrrrEventHandler();
    public delegate Task DrrrRoomEventHandler(DrrrRoom _);
    public delegate Task DrrrUserEventHandler(DrrrUser _);
    public delegate Task DrrrMessageEventHandler(DrrrMessage _);
}