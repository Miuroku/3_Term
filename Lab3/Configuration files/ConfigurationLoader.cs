using System;
using System.IO;

namespace Configuration
{
    class ConfigurationLoader
    {
        // "logMessage" contains all mesages about using default values.
        public string logMessage;
        AllOptions allOptions;
        ConfigurationProvider configprovider;

        public AllOptions GetAllOptions(string configPath)
        {
            allOptions = new AllOptions();
            configprovider = new ConfigurationProvider();
            logMessage = String.Empty;

            try
            {
                TryToFillOptions(configPath);

            }
            catch (Exception e)
            {
                logMessage += e.Message + " We'll use default values.\n";
            }

            Verify();

            return allOptions;
        }

        // Search for config files in "configPath"
        private void TryToFillOptions(string configPath)
        {
            IConfigurationParser parser;

            if (File.Exists(configPath))
            {
                switch (Path.GetExtension(configPath))
                {
                    case ".xml":
                        parser = new XMLParser();
                        break;
                    case ".json":
                        parser = new JsonParser();
                        break;
                    default:
                        throw new Exception("There is no parser.\n");
                }

                try
                {
                    configprovider.GetFilledOptions(allOptions, GetStringFromFile(configPath), parser);
                }
                catch
                {
                    throw new Exception($"File {configPath} can not be parsed\n");
                }
            }
            else if (Directory.Exists(configPath))
            {
                DirectoryInfo configDir = new DirectoryInfo(configPath);

                FileInfo[] jsonFiles = configDir.GetFiles("*.json");
                parser = new JsonParser();

                foreach (var jsonFile in jsonFiles)
                {
                    try
                    {
                        configprovider.GetFilledOptions(allOptions, GetStringFromFile(jsonFile.FullName), parser);
                        logMessage += $"Use configuration from file : {jsonFile.FullName}\n";
                        return;
                    }
                    catch { }
                }

                FileInfo[] XMLFiles = configDir.GetFiles("*.xml");
                parser = new XMLParser();

                foreach (var XMLFile in XMLFiles)
                {
                    try
                    {
                        configprovider.GetFilledOptions(allOptions, GetStringFromFile(XMLFile.FullName), parser);
                        logMessage += $"Use configuration from file : {XMLFile.FullName}\n";

                        return;
                    }
                    catch { }
                }
                throw new Exception($"There is no valid config. file in the directory {configPath}\n");
            }
        }

        // Checks options for current values and set defaults if neccessary.
        private void Verify()
        {

            if (allOptions.archiveOptions.archiveDirectoryPath == null || !Directory.Exists(allOptions.archiveOptions.archiveDirectoryPath))
            {
                logMessage += "\tarchiveDirectoryPath not found -> default value used\n";
                allOptions.archiveOptions.archiveDirectoryPath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\TargetDirectory\archive";
            }            

            if (allOptions.watcherOptions.sourseDirectoryPath == null || !Directory.Exists(allOptions.watcherOptions.sourseDirectoryPath))
            {
                logMessage += "\tsourseDirectoryPath not found -> default value used\n";
                allOptions.watcherOptions.sourseDirectoryPath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\SourceDirectory";
            }            

            if (allOptions.watcherOptions.targetDirectoryPath == null || !Directory.Exists(allOptions.watcherOptions.targetDirectoryPath))
            {
                logMessage += "\ttargetDirectoryPath not found -> default value used\n";
                allOptions.watcherOptions.targetDirectoryPath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\TargetDirectory\FromSourceDirectory";
            }           

            if (allOptions.loggerOptions.logFilePath == null || !File.Exists(allOptions.loggerOptions.logFilePath))
            {
                logMessage += "\tLogFilePath has not found -> default value used\n";
                allOptions.loggerOptions.logFilePath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\TargetDirectory\log.txt";
            }           

        }

        static string GetStringFromFile(string path)
        {
            string response = String.Empty;

            using (StreamReader sr = new StreamReader(path))
            {
                response = sr.ReadToEnd();
            }

            return response;
        }

    }
}