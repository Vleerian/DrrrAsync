using Newtonsoft.Json;
using System.Collections.Generic;

using DrrrAsyncBot.Permission;

namespace DrrrAsyncBot.Objects
{
    public class DrrrBotConfig
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("room")]
        public DrrrRoomConfig Room;
        [JsonProperty("prefix")]
        public string CommandSignal;
        [JsonProperty("icon")]
        public string Icon;
        [JsonProperty("permissions")]
        public Dictionary<string, PermLevel> Permissions;

        [JsonProperty("proxy_uri")]
        public string ProxyURI;
        [JsonProperty("proxy_port")]
        public int ProxyPort;

        public bool Ready
        {
            get
            {
                if (Name != null && Room != null && CommandSignal != null && Icon != null)
                    return true;
                return false;
            }
        }
    }

    public class DrrrMessageConfig
    {
        public string Message;
        public string Username;
        public string Url;
        public bool Direct;
    }

    public class DrrrRoomConfig
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("language")]
        public string Language;
        [JsonProperty("adult")]
        public bool Adult;
        [JsonProperty("music")]
        public bool Music;
    }

    public class DrrrUserConfig
    {
        public string Name;
        public string Tripcode;
        public DrrrIcon Icon;
        public string Device;
    }
}
