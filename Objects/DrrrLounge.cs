using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DrrrAsyncBot.Objects
{
    [Serializable]
    public class DrrrLounge
    {
        [JsonPropertyName("redirect")]
        public string redirect;
        [JsonPropertyName("create_room_url")]
        public string create_room_url;
        [JsonPropertyName("active_user")]
        public int Active_Users;

        [JsonPropertyName("rooms")]
        public List<DrrrRoom> Rooms;
        [JsonPropertyName("profile")]
        public DrrrUser Profile;
    }
}