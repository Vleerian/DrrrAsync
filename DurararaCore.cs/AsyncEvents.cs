using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrrrAsync
{
    namespace AsyncEvents
    {
        public delegate Task AsyncEventHandler();
        public delegate Task AsyncEventHandler<T>(T e) where T : DrrrAsyncEventArgs;

        public class DrrrAsyncEventArgs
        {
            public bool Handled { get; set; }
        }

        public class DrrrAsyncEvent
        {
            private List<AsyncEventHandler> Handlers;

            public async Task InvokeAsync()
            {
                if (!Handlers.Any())
                    return;
                foreach (var handler in Handlers)
                {
                    await handler().ConfigureAwait(false);
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

            public async Task InvokeAsync(T e)
            {
                if (!Handlers.Any())
                    return;
                foreach (var handler in Handlers)
                {
                    await handler(e).ConfigureAwait(false);
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
}
