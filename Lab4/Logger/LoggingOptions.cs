namespace Logger
{
    public class LoggingOptions
    {
        public DataAccessLayer.Options.ConnectionOptions ConnectionOptions { get; set; } = new DataAccessLayer.Options.ConnectionOptions();
        public bool LoggingEnabled{ get; set; } = true;

        public LoggingOptions()
        {

        }
    }
}