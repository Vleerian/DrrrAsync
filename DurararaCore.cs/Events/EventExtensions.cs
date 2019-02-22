using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Events
{
    public static class EventExtensions
    {
        public static async Task FireEventAsync<T>(this T e, object Sender, params object[] args) where T : Delegate
        {
            var invokationList = e.GetInvocationList();
            var handlerTasks = new List<Task>();

            object[] New = new object[args.Length + 1];
            New[0] = Sender;
            for (int i = 0; i < args.Length; i++)
                New[i + 1] = args[i];            

            foreach (var invokation in invokationList)
                handlerTasks.Add((Task) invokation.DynamicInvoke(New));
            await Task.WhenAll(handlerTasks);
        }
    }
}
