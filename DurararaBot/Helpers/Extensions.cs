using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

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

        public static bool Has<T>(this MemberInfo member) where T : Attribute => member.Has<T>(out _);
        public static bool Has<T>(this MemberInfo member, out T foundAttribute) where T : Attribute
        {
            foundAttribute = member.GetCustomAttribute<T>();
            if (foundAttribute != null)
                return true;
            return false;
        }

        public static bool Has<T>(this Type type) where T : Attribute => type.Has<T>(out _);
        public static bool Has<T>(this Type type, out T foundAttribute) where T : Attribute
        {
            foundAttribute = type.GetCustomAttribute<T>();
            if (foundAttribute != null)
                return true;
            return false;
        }
    }
}
