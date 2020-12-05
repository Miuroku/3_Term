using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    public class AllOptions
    {
        // Creating objects.
        public EncryptionOptions encryptionOptions = new EncryptionOptions();
        public WatcherOptions watcherOptions = new WatcherOptions();
        public ArchiveOptions archiveOptions = new ArchiveOptions();
        public LoggerOptions loggerOptions = new LoggerOptions();

        public class EncryptionOptions
        {
            public bool enableEncryption = true;
        }

        public class LoggerOptions
        {
            public bool enableLogging = true;
            public string logFilePath = null;
        }

        public class ArchiveOptions
        {
            public bool enableArchivation = true;
            public string archiveDirectoryPath = null;
        }

        public class WatcherOptions
        {
            public string sourseDirectoryPath = null;
            public string targetDirectoryPath = null;
        }
    }
}

