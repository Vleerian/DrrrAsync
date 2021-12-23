using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

using DrrrAsyncBot.Permission;

namespace DrrrAsyncBot.Objects
{
    public class DrrrBotConfig
    {
        [JsonPropertyName("name")]
        public string Name;
        [JsonPropertyName("room")]
        public DrrrRoomConfig Room;
        [JsonPropertyName("prefix")]
        public string CommandSignal;
        [JsonPropertyName("icon")]
        public string Icon;
        [JsonPropertyName("permissions")]
        public Dictionary<string, PermLevel> Permissions;
        [JsonPropertyName("FlareSolverServer")]
        public string FlareSolver;

        [JsonPropertyName("proxy_uri")]
        public string ProxyURI;
        [JsonPropertyName("proxy_port")]
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
        [JsonPropertyName("name")]
        public string Name;
        [JsonPropertyName("description")]
        public string Description;
        [JsonPropertyName("limit")]
        public int Limit;
        [JsonPropertyName("language")]
        public string Language;
        [JsonPropertyName("adult")]
        public bool Adult;
        [JsonPropertyName("music")]
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
