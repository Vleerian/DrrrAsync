using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrrrAsync.AsyncEvents
{
    //Delegates used by AsyncEvent
    public delegate Task AsyncEventHandler();
    public delegate Task AsyncEventHandler<T>(T e) where T : DrrrAsyncEventArgs;

    public class DrrrAsyncEvent : DrrrAsyncEvent<DrrrAsyncEventArgs>
    {
        public Task InvokeAsync() => InvokeAsync(new DrrrAsyncEventArgs());
    }

    public class DrrrAsyncEvent<T> where T : DrrrAsyncEventArgs
    {
        private List<AsyncEventHandler<T>> Handlers;

        public DrrrAsyncEvent() => Handlers = new List<AsyncEventHandler<T>>();

        /// <summary>
        /// Invokes the event and all it's handlers
        /// </summary>
        /// <param name="e">The Event Arguments you want to pass to the handler</param>
        /// <returns></returns>
        public async Task InvokeAsync(T e)
        {
            // Return if there are no event handlers, or this event has already been handled.
            if (!Handlers.Any() || e.Handled)
                return;
            e.Handled = true;

            foreach (var handler in Handlers)
                await handler(e).ConfigureAwait(false);
        }

        /// <summary>
        /// Registers an event handler to this event.
        /// </summary>
        /// <param name="aTask">The EventHandler you want to register</param>
        public void Register(AsyncEventHandler<T> aTask) => Handlers.Add(aTask);

        /// <summary>
        /// Unregisters a handler from this event
        /// </summary>
        /// <param name="aTask">The EventHandler you want to unregister</param>
        public void Unregister(AsyncEventHandler<T> aTask) => Handlers.Remove(aTask);
    }
}
