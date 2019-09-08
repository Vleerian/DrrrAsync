using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.Common;
using System.Collections;
using Newtonsoft.Json;

namespace DrrrBot.Core
{
    public partial class ServiceHandler
    {
        private static ServiceHandler _instance;
        public static ServiceHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ServiceHandler();
                return _instance;
            }
        }

        static List<string> firstNames;
        public static List<string> FirstNames
        {
            get
            {
                if (firstNames == null)
                {
                    var json = "";
                    using (var sr = new StreamReader(@"FirstNames.json", new UTF8Encoding(false)))
                        json = sr.ReadToEnd();
                    firstNames = JsonConvert.DeserializeObject<List<string>>(json);
                }
                return firstNames;
            }
        }

        static List<string> surNames;
        public static List<string> SurNames
        {
            get
            {
                if (surNames == null)
                {
                    var json = "";
                    using (var sr = new StreamReader(@"SurNames.json", new UTF8Encoding(false)))
                        json = sr.ReadToEnd();
                    surNames = JsonConvert.DeserializeObject<List<string>>(json);
                }
                return firstNames;
            }
        }

        //========================================//
        //         Random Number Related          //
        //========================================//
        private Random _random;
        public Random RNG
        {
            get
            {
                if (_random == null)
                    _random = new Random();
                return _random;
            }
        }
    }

    public struct DictDefiniton
    {
        public string Word, Type, Definition;
    }
}
