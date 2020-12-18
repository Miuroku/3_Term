  
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace XmlGenerator
{
    public class XmlGenerator
    {
        DataAccessLayer.Options.SendingOptions sendingOptions;

        public XmlGenerator(DataAccessLayer.Options.SendingOptions options)
        {
            sendingOptions = options;
            Directory.CreateDirectory(sendingOptions.TargetDirectory);
        }

        public void CreateXML(List<PersonInfo> list)
        {
            XmlSerializer xml = new XmlSerializer(list.GetType());

            using (FileStream fs = new FileStream(Path.Combine(sendingOptions.TargetDirectory, "ourList.xml"), FileMode.Create))
            {
                xml.Serialize(fs, list);
            }
        }
    }
}