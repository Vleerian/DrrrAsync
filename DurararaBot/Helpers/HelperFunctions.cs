using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace DrrrAsync.Bot
{
    public static class HelperFunctions
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


    }
}
