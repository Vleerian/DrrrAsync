using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrrrAsync.Events
{
    public static class EventExtensions
    {
        public static async Task FireEventAsync<T>(this T e, object Sender, params object[] args) where T : Delegate
        {
            var invocationList = e.GetInvocationList();
            var handlerTasks = new List<Task>();

            object[] New = new object[args.Length + 1];
            New[0] = Sender;
            Array.Copy(args, New, args.Length);  

            foreach (var invocation in invocationList)
                handlerTasks.Add((Task)invocation.DynamicInvoke(New));
            await Task.WhenAll(handlerTasks);
        }
    }
}
