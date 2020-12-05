using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileWatcher
{
    class Logger
    {
        bool enableLogging;
        string logPath;
        object obj = new object();

        public Logger(string logPath, bool enableLogging)
        {
            this.enableLogging = enableLogging;
            this.logPath = logPath;            
        }

        public void AddEntry(string entry)
        {
            if (enableLogging)
            {
                // lock(obj) - is using for sync. threads.
                // obj - объект-заглушка, объект obj блокируется и работать может только один поток.
                lock (obj)
                {
                    entry = GetDateString() + entry;
                    // "true" means we'll append entry.
                    using (StreamWriter writer = new StreamWriter(logPath, true))
                    {
                        writer.WriteLine(entry);                        
                    }
                }                
            }            
        }

        public static string GetDateString()
        {
            string date = String.Empty;

            try
            {
                date = $"[{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}]";
            }
            catch { }

            return date;
        }
    }
}
