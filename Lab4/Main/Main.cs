using Converter;
using XmlGenerator;
using DataAccessLayer.Models;
using DataAccessLayer.Options;
using Logger;
using OptionsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    class Program
    {
        static void Main(string[] args)
        {

            IParser ourParser = new Converter.Converter();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            OptionsManager<DataAccessOptions> optionsManager = new OptionsManager<DataAccessOptions>(path, ourParser);
            LoggingOptions loggingOptions = new LoggingOptions();

            // Getting options using OptionsManager.
            loggingOptions.ConnectionOptions = optionsManager.GetOptions<ConnectionOptions>() as ConnectionOptions;
            SendingOptions sendingOptions = optionsManager.GetOptions<SendingOptions>() as SendingOptions;
            ConnectionOptions connectionOptions = optionsManager.GetOptions<ConnectionOptions>() as ConnectionOptions;
            ILogger logger = new Logger.Logger(loggingOptions, ourParser);
            ServiceLayer.ServiceLayer sl = new ServiceLayer.ServiceLayer(connectionOptions, ourParser);            
            logger.Log(optionsManager.log);

            // Getting info from DB.            
            logger.Log("Pulling of the data has been started...");
            var people = sl.GetPersonInfoList(200);
            logger.Log("Pulling of the data has been done successfully!");
            
            // Creating XMl file.
            XmlGenerator.XmlGenerator generator = new XmlGenerator.XmlGenerator(sendingOptions);
            generator.CreateXML(people);
            logger.Log("Xml file was created successfully");
        }

    }
}