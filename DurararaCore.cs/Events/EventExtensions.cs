using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Events
{
    public static class EventExtensions
    {
        public static async Task FireEventAsync<T>(this T e, params object[] args) where T : Delegate
        {
            var invokationList = e.GetInvocationList();
            var handlerTasks = new List<Task>();

            foreach (var invokation in invokationList)
                handlerTasks.Add((Task) invokation.DynamicInvoke(args));
            await Task.WhenAll(handlerTasks);
        }
    }
}
