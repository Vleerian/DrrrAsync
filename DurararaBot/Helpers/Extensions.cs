using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace DrrrAsync.Bot
{
    public static class Extensions
    {
        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target)
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

        private static bool Has<T>(dynamic attributeHolder, out T foundAttribute) where T : Attribute
        {
            foundAttribute = attributeHolder.GetCustomAttribute<T>();
            if (foundAttribute != null)
                return true;
            return false;
        }

        public static bool Has<T>(this MemberInfo member, out T foundAttribute) where T : Attribute =>
            Has(member, out foundAttribute);
        public static bool Has<T>(this MemberInfo member) where T : Attribute =>
            member.Has<T>(out _);
        public static bool Has<T>(this Type type, out T foundAttribute) where T : Attribute =>
            Has(type, out foundAttribute);
        public static bool Has<T>(this Type type) where T : Attribute =>
            type.Has<T>(out _);
    }
}
