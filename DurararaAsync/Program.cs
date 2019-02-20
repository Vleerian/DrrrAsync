using System;
using System.Threading.Tasks;
using DrrrAsync.Objects;

namespace DrrrAsync
{

    class Program
    {
        static async Task Main(string[] args)
        {
            DrrrClient C = new DrrrClient("Welne Oren", "kuromu-2x");
            await C.Login();

            foreach (DrrrRoom Room in await C.GetLounge())
            {
                if (Room.Name == "White Snake Bar18+") ;
                    await C.JoinRoom(Room.RoomId);
            }
            C.SendMessage("Heyylmao").GetAwaiter().GetResult();
            System.Threading.Thread.Sleep(3000);
            C.LeaveRoom().GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}
