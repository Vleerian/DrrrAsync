using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a user on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrUser
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("uid")]
        public string UID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("tripcode")]
        public string Tripcode;
        [JsonProperty("icon")]
        public string Icon;
        [JsonProperty("device")]
        public string Device;
        [JsonProperty("loginedAt")]
        public string LoggedIn;
        [JsonProperty("admin")]
        public bool Admin;
    }
}
