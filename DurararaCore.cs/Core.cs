using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    public interface IClient
    {
        //User Defined
        string Name { get; }
        string Icon { get; }

        //Site-Defined
        string ID { get; }
        DrrrUser User { get; }
        DrrrRoom Room { get; }

        //Client State
        bool LoggedIn { get; }

        //Client state-changing functions
        Task<bool> Login();
        Task<DrrrRoom> JoinRoom(string RoomId);
        Task<DrrrRoom> MakeRoom(DrrrRoom aRoom);
        Task<byte[]> LeaveRoom(DrrrUser aUser);

        //Functions to retrieve data from the site
        Task<DrrrRoom[]> GetLounge();
        Task<DrrrRoom> GetRoom();

        //Functions that cause changes on the site
        Task<byte[]> GiveHost(DrrrUser aUser);
        Task<byte[]> Ban(DrrrUser aUser);
        Task<byte[]> Kick(DrrrUser aUser);
        Task<byte[]> SendMessage(string Message, string Url = "");
    }
}
