using System;

using DrrrAsyncBot.Objects;
using DrrrAsyncBot.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using DrrrAsyncBot.Permission;
using DrrrAsyncBot.Helpers;
using System.Drawing;
using System.Linq;

namespace DrrrAsyncBot
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
