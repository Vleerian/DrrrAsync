using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DrrrAsyncBot;
using DrrrAsyncBot.Core;
using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Logging;

namespace DrrrAsyncBot.Extensions.LoungeMonitor
{
    public static class LoungeMonitorExtension
    {
        private static async Task CompareRoom(this DrrrClient client, DrrrRoom oldRoom, DrrrRoom newRoom)
        {
            var TaskList = new List<Task>();
            try
            {
                // Create the user joined tasks
                var NewUsers = newRoom.Users.Where(User => !oldRoom.Users.Any(U => U.Name == User.Name))
                    .Select(User => client.On_UserJoined?.InvokeAsync(client, new AsyncUserUpdateEventArgs(newRoom, User) { type = LogLevel.UserJoin}));
                
                // Create the user left tasks
                var OldUsers = oldRoom.Users.Where(User => !newRoom.Users.Any(U => U.Name == User.Name))
                    .Select(User => client.On_UserLeft?.InvokeAsync(client, new AsyncUserUpdateEventArgs(newRoom, User) { type = LogLevel.UserJoin}));
                
                // Add both to the task list
                TaskList.AddRange(NewUsers);
                TaskList.AddRange(OldUsers);
            }
            catch(Exception e)
            {
                client.Logger.Log(LogLevel.Error, "Exception encountered in CheckRoom", e);
            }

            // Add events for host changes, room name changes, and description changes
            try{
                if(newRoom.Host != null && newRoom.Host.ID != oldRoom.Host.ID)
                    TaskList.Add(client.On_NewHost?.InvokeAsync(client, new AsyncRoomUpdateEventArgs(oldRoom, newRoom) { type = LogLevel.NewHost }));
                if (newRoom.Name != oldRoom.Name)
                    TaskList.Add(client.On_NewName?.InvokeAsync(client, new AsyncRoomUpdateEventArgs(oldRoom, newRoom) { type = LogLevel.NewName }));
                if (newRoom.Description != oldRoom.Description)
                    TaskList.Add(client.On_NewDescription?.InvokeAsync(client, new AsyncRoomUpdateEventArgs(oldRoom, newRoom) { type = LogLevel.NewDesc }));
            }
            catch(Exception e)
            {
                client.Logger.Log(LogLevel.Error, "Exception encountered registering room event.", e);
            }

            await Task.WhenAll(TaskList);
        }

        public static async Task RunLoungeWatchdog(this DrrrClient client)
        {
            // Get the two lounge states to compare
            var OldLounge = client.Lounge.Rooms;
            var NewLounge = await client.GetLounge();

            // Create the list of tasks to be executed
            var Tasks = new List<Task>();

            // Iterate through the old lounge
            foreach(var Room in OldLounge)
            {
                var NewRoom = NewLounge.Where(R => R.RoomId == Room.RoomId).FirstOrDefault();
                // If the room does not exist, add a RoomDeleted event call to the task list
                if(NewRoom == default)
                {
                    Tasks.Add(client.On_RoomDeleted?.InvokeAsync(client, new(Room)));
                    continue;
                }
                // Otherwise, add a comparison task to the task list
                Tasks.Add(client.CompareRoom(Room, NewRoom));
            }

            // Get all elements in NewLounge that do not exist in OldLounge
            var NewRoomTasks = NewLounge.Where(New => !OldLounge.Any(Old => Old.RoomId == New.RoomId))
            // Convert it into a collection of NewRoom even calls
                .Select(Room => client.On_NewRoom?.InvokeAsync(client, new(Room)));
            // Merge the NewRoom tasks into the master task list
            Tasks.AddRange(NewRoomTasks);

            await Task.WhenAll(Tasks);
        }
    }
}