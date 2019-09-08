using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrrrBot.Core
{
    public delegate Task AsyncEventHandler(object Sender);
    public delegate Task AsyncEventHandler<T>(object Sender, T e) where T : DrrrAsyncEventArgs;

    public class DrrrAsyncEvent
    {
        private List<AsyncEventHandler> Handlers;

        public DrrrAsyncEvent() =>
            Handlers = new List<AsyncEventHandler>();

        public async Task InvokeAsync(object Sender)
        {
            if (!Handlers.Any())
                return;
            foreach (var handler in Handlers)
            {
                await handler(Sender).ConfigureAwait(false);
            }
        }

        public void Register(AsyncEventHandler aTask)
        {
            Handlers.Add(aTask);
        }

        public void Unregister(AsyncEventHandler aTask)
        {
            Handlers.Remove(aTask);
        }

    }

    public class DrrrAsyncEvent<T> where T : DrrrAsyncEventArgs
    {
        private List<AsyncEventHandler<T>> Handlers;

        public DrrrAsyncEvent() =>
            Handlers = new List<AsyncEventHandler<T>>();

        public async Task InvokeAsync(object Sender, T e)
        {
            if (!Handlers.Any())
                return;
            foreach (var handler in Handlers)
            {
                await handler(Sender, e).ConfigureAwait(false);
            }
        }

        public void Register(AsyncEventHandler<T> aTask)
        {
            Handlers.Add(aTask);
        }

        public void Unregister(AsyncEventHandler<T> aTask)
        {
            Handlers.Remove(aTask);
        }

    }
}
