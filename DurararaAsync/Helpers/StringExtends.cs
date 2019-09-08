using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrrrBot.Helpers
{
    static class StringHelpers
    {
        public static string WordWrap(string text, int maxLineLength)
        {
            var List = new List<string>();

            var whitespace = new List<char>() { ' ', '\r', '\n', '\t' };
            while (maxLineLength < text.Length)
            {
                var lastSpace = 0;
                for (int i = 0; i < maxLineLength; i++)
                {
                    if (whitespace.Contains(text[i]))
                        lastSpace = i;
                }
                if (text.Trim() != string.Empty)
                    List.Add(Utilities.SnipText(ref text, lastSpace).Trim());

            }
            if (text.Trim() != string.Empty)
                List.Add(text.Trim());
            return string.Join("\n", List);
        }

        public static string Center(this string source, int length, char padchar = ' ')
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, padchar).PadRight(length, padchar);

        }

        public static byte[] GetBytes(this string s) => Encoding.UTF8.GetBytes(s);
        public static int ByteLength(this string s) => Encoding.UTF8.GetBytes(s).Length;
    }
}
