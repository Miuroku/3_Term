using System;
using System.IO;
using System.Threading;
using Configuration;

namespace FileWatcher
{
    class Watcher
    {
        // Path to file or folder where configuration files could exist.
        string configurationPath = @"C:\Users\supay\Desktop\Labs_CS"; 

        bool enableArhivation;
        bool enableLogging;
        bool enableEncryption;
        string sourceDirectory = String.Empty;
        string targetDirectory = String.Empty;
        string archiveDirectory = String.Empty;
        string loggerPath = String.Empty;
        object obj;

        FileSystemWatcher watcher;
        Logger logger;
        bool enabled = true;

        public Watcher()
        {
            ConfigurationLoader configLoader = new ConfigurationLoader();
            // Getting all possible options.            
            AllOptions allOptions = configLoader.GetAllOptions(configurationPath);

            // Load all options.
            sourceDirectory = allOptions.watcherOptions.sourseDirectoryPath;
            targetDirectory = allOptions.watcherOptions.targetDirectoryPath;
            archiveDirectory = allOptions.archiveOptions.archiveDirectoryPath;
            loggerPath = allOptions.loggerOptions.logFilePath;
            enableArhivation = allOptions.archiveOptions.enableArchivation;
            enableEncryption = allOptions.encryptionOptions.enableEncryption;
            enableLogging = allOptions.loggerOptions.enableLogging;            

            obj = new object();
            logger = new Logger(loggerPath, enableLogging);

            // Entry about results of getting options.
            logger.AddEntry(configLoader.logMessage);

            watcher = new FileSystemWatcher(sourceDirectory);

            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Created += WatcherCreated;
        }

        public void Start()
        {
            logger.AddEntry("Service starts");

            // Begin watching.
            watcher.EnableRaisingEvents = true;

            while (enabled)
            {
                // Suspends a thread for a second.
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            logger.AddEntry("Service stopped");

            // Stop watching.
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "created";
            string fullFilePath = e.FullPath;

            // Тут вся логика перемещений шифрований и тд.
            fullFilePath = MainActions.RenameBuisnessFile(fullFilePath);

            // Creating tmp directory for saving tmp results.
            string tmpDirName = "tmpDir";
            string tmpDirectory = MainActions.CreateSubDirectory(Path.GetDirectoryName(fullFilePath), tmpDirName);

            string tmpFilePath = Path.Combine(tmpDirectory, Path.GetFileName(fullFilePath));
            File.Copy(fullFilePath, tmpFilePath);

            // 1) Зашифровать.
            string filePathAfterEncryption = tmpFilePath;
            if (enableEncryption)
                filePathAfterEncryption = MainActions.Encrypt(tmpFilePath);

            // 2) Заархивировать.
            string fileNameAfterCompression = filePathAfterEncryption;
            if (enableArhivation)
                fileNameAfterCompression = MainActions.Compress(filePathAfterEncryption);

            // 2.1) Создать врем хранилище "tmpDir" в "TargetDirectory".
            string tmpDir2 = MainActions.CreateSubDirectory(targetDirectory, tmpDirName);

            // 3) Переместить в "tmpDir".
            string filePathAfterMove = MainActions.MoveFileOrFolder(fileNameAfterCompression, tmpDir2);

            // 3.1) Удалить временное хранилище "tmpDirectory" из "SourceDirectory".
            Directory.Delete(tmpDirectory, true);

            // 4) Разархивировать.
            string filePathAfterDecompression = filePathAfterMove;
            if (enableArhivation)
                filePathAfterDecompression = MainActions.DecompressGZip(filePathAfterMove);

            // 5) Расшифровать.
            string filePathAfterDecryption = filePathAfterDecompression;
            if (enableEncryption)
            {
                filePathAfterDecryption = MainActions.Decrypt(filePathAfterDecompression);
                //filePathAfterDecryption = MainActions.RenameFile(filePathAfterDecryption, Path.GetFileNameWithoutExtension(fullFilePath));
            }

            // 5.1) Переместить готовый ответ в основное "TargetDirectory".
            string fileAfterMoving = MainActions.MoveFileOrFolder(filePathAfterDecryption, targetDirectory);

            // 5.2) Удалить "tmpDir2".
            Directory.Delete(tmpDir2, true);

            // 6) Копировать в "TargetDirectory\archive" - с точно таким же расположением папок как и в "SourceDirectory".  
            if (enableArhivation) 
            {
                string fileAfterCopying = Path.Combine(archiveDirectory, Path.GetFileName(fileAfterMoving));
                Directory.CreateDirectory(archiveDirectory);
                File.Copy(fileAfterMoving, fileAfterCopying, true);

                MainActions.RenameBuisnessFile(fileAfterCopying);                
            }

            string fileName = Path.GetFileName(fullFilePath);                        

            RecordEntry(fileEvent, fileName);            
        }

        // Writes information about files in the logger.        
        private void RecordEntry(string fileEvent, string fileName)
        {            
            // lock(obj) - is using for sync. threads.
            // obj - объект-заглушка, объект obj блокируется и работать может только один поток.
            lock (obj)
            {                
                logger.AddEntry($" - File {fileName} {fileEvent}" );
            }
        }
    }
}
