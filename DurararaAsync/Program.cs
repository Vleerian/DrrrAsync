using System;
using System.Threading.Tasks;
using DrrrAsync.Objects;

namespace DrrrAsync
{

    class Program
    {
        static async Task Main(string[] args)
        {
            DrrrClient C = new DrrrClient { Name = "Welne Oren", Icon = "kuromu-2x" };
            await C.Login();
            while (true) ;
            foreach (DrrrRoom Room in await C.GetLounge())
            {
                if (Room.Name == "White Snake Bar18+") ;
                    await C.JoinRoom(Room.RoomId);
            }
            await C.SendMessage("Heyylmao");
            System.Threading.Thread.Sleep(3000);
            await C.LeaveRoom();
            Console.ReadKey();
        }
    }
}
