using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    public partial class DrrrClient
    {
        /// <summary>
        /// Creates a room on Drrr.com
        /// </summary>
        /// <param name="aRoom">The DrrrRoom object with information set about the room you want to create.</param>
        /// <returns>All available data about the created room.</returns>
        public async Task<DrrrRoom> MakeRoom(DrrrRoom aRoom)
        {
            // Send a POST to create the room.
            Uri WebAddress = new Uri("https://drrr.com/create_room/?");
            byte[] response = await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "name", aRoom.Name },
                { "description", aRoom.Description },
                { "limit", aRoom.Limit.ToString() },
                { "language", aRoom.Language },
                { "adult", aRoom.AdultRoom.ToString() },
            });

            // Retrieve room data
            WebAddress = new Uri($"http://drrr.com/room/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            await OnRoomJoin?.InvokeAsync(Room);
            return Room;
        }

        /// <summary>
        /// Joins a room on Drrr.com
        /// </summary>
        /// <param name="RoomId">The ID of the room, as found in GetLounge</param>
        /// <returns>A DrrrRoom object containing all the available data for that room.</returns>
        public async Task<DrrrRoom> JoinRoom(string RoomId)
        {
            // Join the room
            Logger.Info($"Joining room: {RoomId}");
            Uri WebAddress = new Uri($"http://drrr.com/room/?id={RoomId}");
            Logger.Debug($"URL: {WebAddress.AbsoluteUri}");
            await WebClient.DownloadStringTaskAsync(WebAddress);

            // Download and parse room data
            WebAddress = new Uri($"https://drrr.com/json.php?fast=1");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));
            Room = new DrrrRoom(RoomData);

            await OnRoomJoin?.InvokeAsync(Room);
            return Room;
        }

        /// <summary>
        /// Retrieves a room's updated data from Drrr.com
        /// </summary>
        /// <returns>A DrrrRoom object created with the update data.</returns>
        public async Task<DrrrRoom> GetRoom()
        {
            // Retrieve the room's data from the update api endpoint
            Uri WebAddress = new Uri($"http://drrr.com/json.php?update={Room.Update}");
            JObject RoomData = JObject.Parse(await WebClient.DownloadStringTaskAsync(WebAddress));

            // Fire an event for each parsed message
            // Could this be moved to the DrrrRoom or DrrrMessage constructor in a clean way?
            foreach (DrrrMessage Mesg in Room.UpdateRoom(RoomData))
            {
                await OnMessage?.InvokeAsync(Mesg);
                // If it's a direct message, fire the OnDirectMessage event
                if (Mesg.Secret)
                    await OnDirectMessage?.InvokeAsync(Mesg);
            }

            return Room;
        }

        /// <summary>
        /// Leaves a room.
        /// NOTE: This method does not check whether or not you left successfully.
        /// If you have been in a room less than 30 seconds, you will recieve a 403 error.
        /// </summary>
        /// <returns>The raw byte[] data returned from the web request.</returns>
        public async Task<byte[]> LeaveRoom()
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "leave", "leave" }
            });
        }

        /// <summary>
        /// Gives host to another user.
        /// NOTE: This method does not check whether or not you have host, or if it was transferred succesffully.
        /// </summary>
        /// <param name="aUser">The user you want to give host to.</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> GiveHost(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "new_host", aUser.ID }
            });
        }

        /// <summary>
        /// Bans a user from the room
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully banned.
        /// </summary>
        /// <param name="aUser">The user you want to ban</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> Ban(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "ban", aUser.ID }
            });
        }

        /// <summary>
        /// Kicks a user from the room.
        /// NOTE: This method does not check whether or not you have host, if that user is in the room, or if they were successfully kicked.
        /// </summary>
        /// <param name="aUser">The user you want to kick</param>
        /// <returns>The raw byte[] data returned from the web request</returns>
        public async Task<byte[]> Kick(DrrrUser aUser)
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");
            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "kick", aUser.ID }
            });
        }

        /// <summary>
        /// Sends a message to the room you are currently in.
        /// NOTE: This won't check whether or not you are in a room, and won't protect you from anti-spam measures.
        /// </summary>
        /// <param name="Message">The message you want to send</param>
        /// <param name="Url">The URL (if any) you want to attach.</param>
        /// <returns></returns>
        public async Task<byte[]> SendMessage(string Message, string Url = "")
        {
            Uri WebAddress = new Uri("https://drrr.com/room/?ajax=1");

            return await WebClient.UploadValuesTaskAsync(WebAddress, "POST", new NameValueCollection() {
                { "message", Message },
                { "url",     Url     },
                { "to",      ""      }
            });
        }
    }
}
