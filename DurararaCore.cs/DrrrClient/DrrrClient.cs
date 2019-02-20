using DrrrAsync.Objects;
using DrrrAsync.AsyncEvents;
using DrrrAsync.Logging;

namespace DrrrAsync
{
    public partial class DrrrClient
    {
        private readonly Logger Logger = new Logger { LogLevel = LogLevel.DEBUG, Name = "Client \"DrrrBot\"" };

        // User Defined
        private string name = "DrrrBot";
        public string Name {
            get => name;
            set
            {
                name = value;
                Logger.Name = $"Client \"{name}\"";
            }
        }
        public string Icon { get; set; } = "kuromu-2x";

        // Site-Defined
        public string ID { get; private set; }
        public DrrrUser User { get; private set; }
        public DrrrRoom Room { get; private set; }

        // Client State
        public bool LoggedIn { get; private set; }

        // Client Extensions
        public CookieWebClient WebClient = new CookieWebClient();

        public DrrrAsyncEvent OnLogin = new DrrrAsyncEvent();
        public DrrrAsyncEvent<DrrrRoom> OnRoomJoin = new DrrrAsyncEvent<DrrrRoom>();
        //public event EventHandler<DrrrUser> On_User_Joined  = new DrrrAsyncEvent<DrrrRoom>();
        public DrrrAsyncEvent<DrrrMessage> OnMessage = new DrrrAsyncEvent<DrrrMessage>();
        public DrrrAsyncEvent<DrrrMessage> OnDirectMessage = new DrrrAsyncEvent<DrrrMessage>();
    }
}
