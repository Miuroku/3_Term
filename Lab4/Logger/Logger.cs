using Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Logger
{
    public class Logger : ILogger
    {
        LoggingOptions loggingOptions;
        DataAccessLayer.DataAccessLayer dataAccessLayer;

        public Logger()
        {

        }

        public Logger(LoggingOptions options, IParser parser)
        {
            loggingOptions = options;
            dataAccessLayer = new DataAccessLayer.DataAccessLayer(loggingOptions.ConnectionOptions, parser);
        }        

        public void Log(string msg)
        {
            if (loggingOptions.LoggingEnabled)
            {
                dal.Log(DateTime.Now, msg);
            }
        }

        public void Start()
        {

        }

        public void Stop()
        {
            loggingOptions.LoggingEnabled = false;
        }
    }
}