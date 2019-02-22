using System;
using System.Collections.Generic;
using System.Reflection;

namespace Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Has wraps GetCustomAttribute to simplify fetching custom attributes.
        /// It primarily removes the need to check for null in code.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <param name="aAttribute">The output of the attribute</param>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<A>(this MethodInfo Info, out A aAttribute) where A : Attribute
        {
            aAttribute = Info.GetCustomAttribute<A>();
            if (aAttribute == null)
                return false;
            return true;
        }

        /// <summary>
        /// Has wraps GetCustomAttribute to simplify fetching custom attributes.
        /// It primarily removes the need to check for null in code.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <param name="aAttribute">The output of the attribute</param>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<A>(this Type obj, out A aAttribute) where A : Attribute
        {
            aAttribute = obj.GetCustomAttribute<A>();
            if (aAttribute == null)
                return false;
            return true;
        }

        /// <summary>
        /// Has wraps GetCustomAttribute to simplify checking for custom attributes.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<A>(this MethodInfo Info) where A : Attribute
        {
            if (Info.GetCustomAttribute<A>() == null)
                return false;
            return true;
        }

        /// <summary>
        /// Has wraps GetCustomAttribute to simplify checking for custom attributes.
        /// </summary>
        /// <typeparam name="A">The attribute you want to get</typeparam>
        /// <returns>Whether or not an attribute was found</returns>
        public static bool Has<A>(this Type obj) where A : Attribute
        {
            if (obj.GetCustomAttribute<A>() != null)
                return true;
            return false;
        }

        /// <summary>
        /// Removes an item from the array, and returns the removed item
        /// </summary>
        /// <param name="index">The index of the item you want removed. Default: last item</param>
        /// <returns>The removed item</returns>
        public static T Pop<T>(this T[] source, int index = -1)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            T item = source[index];
            source = dest;
            return item;
        }

        /// <summary>
        /// Removes an item from the list, and returns the removed item
        /// </summary>
        /// <param name="index">The index of the item you want removed. Default: last item</param>
        /// <returns>The removed item</returns>
        public static T Pop<T>(this List<T> source, int index = -1)
        {
            T item = source[index];
            source.RemoveAt(index > 0 ? index : source.Count - 1);

            return item;
        }
    }
}