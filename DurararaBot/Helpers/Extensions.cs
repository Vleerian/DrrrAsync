using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DrrrAsync.Bot
{
    public static class Extensions
    {
        /// <summary>
        /// Creates a dynamically typed delegate from just the 
        /// method info provided on the specified target.
        /// </summary>
        /// <param name="methodInfo">The method to create a delegate for</param>
        /// <param name="target">The target to create the delegate for, if non-static.</param>
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target = null)
        {
            var getType = methodInfo.ReturnType == typeof(void)
                ? (Func<Type[], Type>) Expression.GetActionType
                : (Func<Type[], Type>) Expression.GetFuncType;

            var types = methodInfo.GetParameters().Select(p => p.ParameterType);
            if (getType == Expression.GetFuncType)
                types = types.Concat(new[] { methodInfo.ReturnType });

            if (methodInfo.IsStatic)
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            else return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
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
            source.RemoveAt(index > -1 ? index : source.Count - 1);

            return item;
        }
    }
}
