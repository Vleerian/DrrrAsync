using System;

namespace DrrrAsync.Bot
{
    public class EventHandlerAttribute : Attribute
    {
        public readonly string EventName;
        
        public EventHandlerAttribute(string eventName) =>
            EventName = eventName;
    }
}
