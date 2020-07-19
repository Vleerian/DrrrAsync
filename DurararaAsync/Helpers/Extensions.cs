using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using log4net;

namespace DrrrAsyncBot.Helpers
{
    public static class logExtensions
    {
        public static readonly log4net.Core.Level doneLevel = new log4net.Core.Level(40000, "Done");

        public static void Done (this ILog log, string message) =>
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                doneLevel, message, null);

        public static void Done (this ILog log, string message, Exception exception) =>
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                doneLevel, message, exception);

        public static void DoneFormat (this ILog log, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                doneLevel, formattedMessage, null);
        }
    }

    /// <summary>
    /// Common extension methods
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// An Async version of ForEach. Iterates through an IEnumerable, executing a function on each item.
        /// </summary>
        /// <typeparam name="T">The Type stored in the IEnumerable</typeparam>
        /// <param name="Enumerable">The IEnumerable being iterated over</param>
        /// <param name="func">The function being executed on each item</param>
        public static async Task ForEachAsync<T>(this IEnumerable<T> Enumerable, Func<T, Task> func)
        {
            foreach (var item in Enumerable)
                await func(item);
        }

        /// <summary>
        /// Iterates through an IEnumerable, executing a function on each item.
        /// </summary>
        /// <typeparam name="T">The Type stored in the IEnumerable</typeparam>
        /// <param name="Enumerable">The IEnumerable being iterated over</param>
        /// <param name="func">The function being executed on each item</param>
        public static void ForEach<T>(this IEnumerable<T> Enumerable, Action<T> Action)
        {
            foreach (var item in Enumerable)
                Action(item);
        }


        /// <summary>
        /// Has wraps GetCustomAttribute to simplify checking for custom attributes.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<T>(this MemberInfo member) where T : Attribute => member.Has<T>(out _);
        /// <summary>
        /// Has wraps GetCustomAttribute to simplify fetching custom attributes.
        /// It primarily removes the need to check for null in code.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <param name="aAttribute">The output of the attribute</param>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<T>(this MemberInfo member, out T foundAttribute) where T : Attribute
        {
            foundAttribute = member.GetCustomAttribute<T>();
            if (foundAttribute != null)
                return true;
            return false;
        }

        /// <summary>
        /// Has wraps GetCustomAttribute to simplify checking for custom attributes.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<T>(this Type type) where T : Attribute => type.Has<T>(out _);
        /// <summary>
        /// Has wraps GetCustomAttribute to simplify fetching custom attributes.
        /// It primarily removes the need to check for null in code.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <param name="aAttribute">The output of the attribute</param>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<T>(this Type type, out T foundAttribute) where T : Attribute
        {
            foundAttribute = type.GetCustomAttribute<T>();
            if (foundAttribute != null)
                return true;
            return false;
        }

        /// <summary>
        /// Removes an item from the list, and returns the removed item
        /// </summary>
        /// <param name="index">The index of the item you want removed. Default: last item</param>
        /// <returns>The removed item</returns>
        public static T Pop<T>(this List<T> source, int index = -1)
        {
            T item = source[index];
            source.RemoveAt(index > -1 ? index : source.Count - 1);

            return item;
        }

        /// <summary>
        /// Centers a string using padding on both sides
        /// </summary>
        /// <param name="source">The string being padded</param>
        /// <param name="length">The total string length, including padding</param>
        /// <param name="padchar">The character to pad with</param>
        /// <returns>The padded string</returns>
        public static string Center(this string source, int length, char padchar = ' ')
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft, padchar).PadRight(length, padchar);

        }
    }
}
