using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

using DrrrAsync.Core;

namespace DrrrAsync.Objects
{
    public class DrrrBotConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("room")]
        public DrrrRoomConfig Room { get; set; }
        [JsonPropertyName("prefix")]
        public string CommandSignal { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("permissions")]
        public Dictionary<string, PermLevel> Permissions { get; set; }

        [JsonPropertyName("proxy_uri")]
        public string ProxyURI { get; set; }
        [JsonPropertyName("proxy_port")]
        public int ProxyPort { get; set; }

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
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("adult")]
        public bool Adult { get; set; }
        [JsonPropertyName("music")]
        public bool Music { get; set; }
    }

    public class DrrrUserConfig
    {
        public string Name;
        public string Tripcode;
        public DrrrIcon Icon;
        public string Device;
    }
}
