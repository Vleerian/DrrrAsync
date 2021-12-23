using System;
using System.Text.Json.Serialization;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A container for information pertaining to a user on Drrr.Com
    /// </summary>
    [Serializable]
    public class DrrrUser
    {
        [JsonPropertyName("id")]
        public string ID;
        [JsonPropertyName("uid")]
        public string UID;
        [JsonPropertyName("name")]
        public string Name;
        [JsonPropertyName("tripcode")]
        public string Tripcode;
        [JsonPropertyName("icon")]
        public string Icon;
        [JsonPropertyName("device")]
        public string Device;
        [JsonPropertyName("loginedAt")]
        public string LoggedIn;
        [JsonPropertyName("admin")]
        public bool Admin;

        public bool HasTripcode()
        {
            if(Tripcode == null)
                return false;
            if(Tripcode == string.Empty)
                return false;
            return true;
        }
        
        public string NiceName()
        {
            string NiceName = Name;
            if(HasTripcode())
                NiceName += $"#{Tripcode}";
            return NiceName;
        }
    }
}
