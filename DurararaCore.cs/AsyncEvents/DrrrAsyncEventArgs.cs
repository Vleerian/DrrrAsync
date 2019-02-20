namespace DrrrAsync.AsyncEvents
{
    //A base class for things sent to DrrrAsyncEventHandlers to inherit from
    public class DrrrAsyncEventArgs
    {
        public bool Handled { get; set; }
    }
}