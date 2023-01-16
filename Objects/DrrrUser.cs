using System;
using System.Text.Json.Serialization;

using DrrrAsync;

namespace DrrrAsync.Objects
{
    /// <summary>
    /// A container for information pertaining to a user on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrUser
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("uid")]
        public string UID { get; set; }
        [JsonPropertyName("name"), JsonConverter(typeof(AutoNumberToStringConverter))]
        public string Name { get; set; }
        [JsonPropertyName("tripcode"), JsonConverter(typeof(AutoNumberToStringConverter))]
        public string Tripcode { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("device")]
        public string Device { get; set; }
        [JsonPropertyName("loginedAt")]
        public double LoggedIn { get; set; }
        [JsonPropertyName("admin")]
        public bool Admin { get; set; }
        [JsonPropertyName("player")]
        public bool? Player { get; set; }
    }
}
