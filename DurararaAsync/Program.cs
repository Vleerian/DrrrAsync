using System;

using DrrrAsync.Objects;

namespace DrrrAsync
{

    class Program
    {
        static void Main(string[] args)
        {
            DrrrClient C = new DrrrClient("Welne Oren", "kuromu-2x");
            C.Login().GetAwaiter().GetResult();

            foreach (DrrrRoom Room in C.GetLounge().GetAwaiter().GetResult())
            {
                if (Room.Name == "White Snake Bar18+")
                    C.JoinRoom(Room.RoomId).GetAwaiter().GetResult();
            }
            C.SendMessage("Heyylmao").GetAwaiter().GetResult();
            System.Threading.Thread.Sleep(3000);
            C.LeaveRoom().GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}
