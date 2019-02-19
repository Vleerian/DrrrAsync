using System;

using DrrrAsync.Objects;

namespace DrrrAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            DrrrClient C = new DrrrClient("NBot", "setton-2x");
            C.Login().GetAwaiter().GetResult();

            foreach (DrrrRoom Room in C.GetLounge().GetAwaiter().GetResult())
            {
                if (Room.Name == "Abyss Bar 18+")
                    /*C.JoinRoom(Room.RoomId).GetAwaiter().GetResult()/**/;
            }

            Console.ReadKey();
        }
    }
}
