using System;

using DrrrBot.Objects;
using DrrrBot.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using DrrrBot.Permission;
using DrrrBot.Helpers;
using System.Drawing;
using System.Linq;

namespace DrrrBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var Tests = new Test();
            Tests.RunTests().Wait();
        }
    }
}
