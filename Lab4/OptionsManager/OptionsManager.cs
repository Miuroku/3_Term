using Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OptionsManager
{
    public class OptionsManager<T> where T : new()
    {
        T JsonOptions;
        T XmlOptions;
        T DefaultOptions = new T();
        public string log = "";
        public bool JsonExists;
        public bool isJsonValid;
        public bool XmlExists;
        public bool isXmlValid;

        public OptionsManager(string path, IParser parser)
        {
            string input;
            string jsonPath = $"{path}\\appsettings.json";
            string xmlPath = $"{path}\\config.xml";
            if (File.Exists(jsonPath))
            {
                JsonExists = true;
                log += "appsettings.json founded ";
                try
                {
                    input = File.ReadAllText(jsonPath);
                    JsonOptions = parser.DeserializeJSON<T>(input);
                    isJsonValid = true;
                    log += "and it's valid.";
                }
                catch
                {
                    log += "but it isn't valid.";
                    isJsonValid = false;
                }
            }
            else
            {
                log += "appsettings.json not found";
                isJsonValid = false;
                JsonExists = false;
            }
            if (File.Exists(xmlPath))
            {
                log += "\nconfig.xml founded ";
                XmlExists = true;
                
                try
                {
                    input = File.ReadAllText(xmlPath);
                    XmlOptions = parser.DeserializeXML<T>(input);
                    isXmlValid = true;
                    log += "and valid.";
                }
                catch
                {
                    log += "but isn't valid.";
                    isXmlValid = false;
                }
            }
            else
            {
                log += "\nconfig.xml not found";
                isXmlValid = false;
                XmlExists = false;
            }
            if (isJsonValid)
            {
                log += "\n\nLoading appsettings.json";
            }
            else
            {
                if (isXmlValid)
                {
                    log += "\n\nLoading config.xml";
                }
                else
                {
                    log += "\n\nBoth configs are invalid. Loading Default config";
                }
            }
        }

        public object GetOptions<T>()
        {
            if (isJsonValid)
            {
                return FindOption<T>(JsonOptions);
            }
            else
            {
                if (isXmlValid)
                {
                    return FindOption<T>(XmlOptions);
                }
                else
                {
                    return FindOption<T>(DefaultOptions);
                }
            }
        }
        object FindOption<T>(object options)
        {
            if (typeof(T) == DefaultOptions.GetType())
            {
                return options;
            }
            string name = typeof(T).Name;
            try
            {
                return options.GetType().GetProperty(name).GetValue(options, null);
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
    }
}