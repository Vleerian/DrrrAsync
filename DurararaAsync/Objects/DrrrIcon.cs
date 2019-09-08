using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrrrBot.Objects
{
    [Serializable]
    public sealed class DrrrIcon
    {
        public static readonly DrrrIcon Setton = new DrrrIcon("setton");
        public static readonly DrrrIcon Bakyura2x = new DrrrIcon("bakyura-2x");
        public static readonly DrrrIcon Ya2x = new DrrrIcon("ya-2x");
        public static readonly DrrrIcon Sharo2x = new DrrrIcon("sharo-2x");
        public static readonly DrrrIcon Kuromu2x = new DrrrIcon("kuromu-2x");
        public static readonly DrrrIcon Kanra = new DrrrIcon("kanra");
        public static readonly DrrrIcon Kakka = new DrrrIcon("kakka");
        public static readonly DrrrIcon Kyo2x = new DrrrIcon("kyo-2x");
        public static readonly DrrrIcon GG = new DrrrIcon("gg");
        public static readonly DrrrIcon Junsui2x = new DrrrIcon("junsui-2x");
        public static readonly DrrrIcon Tanaka2x = new DrrrIcon("tanaka-2x");
        public static readonly DrrrIcon Gaki2x = new DrrrIcon("gaki-2x");
        public static readonly DrrrIcon Kanra2x = new DrrrIcon("kanra-2x");
        public static readonly DrrrIcon Rotchi2x = new DrrrIcon("rotchi-2x");
        public static readonly DrrrIcon Saki2x = new DrrrIcon("saki-2x");
        public static readonly DrrrIcon Zaika = new DrrrIcon("zaika");
        public static readonly DrrrIcon San_2x = new DrrrIcon("san-2x");
        public static readonly DrrrIcon Zawa = new DrrrIcon("zawa");
        public static readonly DrrrIcon Bakyura = new DrrrIcon("bakyura");
        public static readonly DrrrIcon Eight = new DrrrIcon("eight");
        public static readonly DrrrIcon Zaika2x = new DrrrIcon("zaika-2x");
        public static readonly DrrrIcon Setton2x = new DrrrIcon("setton-2x");
        public static readonly DrrrIcon Tanaka = new DrrrIcon("tanaka");

        public string ID { get; }
        private DrrrIcon(string ID) => this.ID = ID;
    }
}
