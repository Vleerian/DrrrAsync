using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DrrrAsyncBot.Objects
{
    [Serializable]
    public class DrrrLounge
    {
        [JsonProperty("redirect")]
        public string redirect;
        [JsonProperty("create_room_url")]
        public string create_room_url;
        [JsonProperty("active_user")]
        public int Active_Users;

        [JsonProperty("rooms")]
        public List<DrrrRoom> Rooms;
        [JsonProperty("profile")]
        public DrrrUser Profile;
    }
}