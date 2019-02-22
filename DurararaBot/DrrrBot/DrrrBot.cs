namespace DrrrAsync.Bot
{
    public partial class DrrrBot : DrrrClient
    {
        public bool Running { get; private set; }
        public string CommandPrefix = "#";
        
        public DrrrBot() : base()
        {

        }
    }
}
