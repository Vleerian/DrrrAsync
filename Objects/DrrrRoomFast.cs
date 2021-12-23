using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DrrrAsyncBot.Objects
{
    /// <summary>
    /// A different version of DrrrRoom for the endpoitns where host is a user id
    /// </summary>
    [Serializable]
    public class DrrrRoomFast : DrrrRoom
    {
        [JsonProperty("host")]
        public string host;

        public new DrrrUser Host
        {
            get =>
                Users.Where(User=>User.ID == host).FirstOrDefault();
        }


    }
}