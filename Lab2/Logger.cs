using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FileWatcherForFun
{
    class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        string logFilePath = String.Empty;
        string targetDirPath = String.Empty;

        // "dirPath" - the path to directory that we woud like to track.
        public Logger(string dirForWatchPath, string logFilePath, string targetDirPath)
        {
            watcher = new FileSystemWatcher(dirForWatchPath);
            this.logFilePath = logFilePath;
            this.targetDirPath = targetDirPath;

            // Only watch text files.
            watcher.Filter = "*.txt";

            // Add event handlers.
            watcher.Created += WatcherCreated;
        }

        public void Start()
        {
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
            // Stop watching.
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void WatcherCreated(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "created";
            string fullFilePath = e.FullPath;

            fullFilePath = MainActions.RenameBuisnessFile(fullFilePath);

            // Creating tmp directory for saving tmp results.
            string tmpDirName = "tmpDir";

            string tmpDirectory = MainActions.CreateSubDirectory(Path.GetDirectoryName(fullFilePath), tmpDirName);

            string tmpFilePath = Path.Combine(tmpDirectory, Path.GetFileName(fullFilePath));
            File.Copy(fullFilePath, tmpFilePath);

            // ----
            // 1) Зашифровать.
            string filePathAfterEncryption = MainActions.Encrypt(tmpFilePath);

            // 2) Заархивировать.
            string fileNameAfterCompression = MainActions.Compress(filePathAfterEncryption);

                // 2.1) Создать врем хранилище "tmpDir" в "TargetDirectory".
                string tmpDir2 = MainActions.CreateSubDirectory(targetDirPath, tmpDirName);

            // 3) Переместить в "tmpDir".
            string filePathAfterMove = MainActions.MoveFileOrFolder(fileNameAfterCompression, tmpDir2);

                // 3.1) Удалить временное хранилище "tmpDirectory" из "SourceDirectory".
                Directory.Delete(tmpDirectory , true);

            // 4) Разархивировать.
            string filePathAfterDecompression = MainActions.DecompressGZip(filePathAfterMove);

            // 5) Расшифровать.
            string filePathAfterDecryption = MainActions.Decrypt(filePathAfterDecompression);
            filePathAfterDecryption = MainActions.RenameFile(filePathAfterDecryption, Path.GetFileNameWithoutExtension(fullFilePath));

                // 5.1) Переместить готовый ответ в основное "TargetDirectory".
                MainActions.MoveFileOrFolder(filePathAfterDecryption, targetDirPath);

                // 5.2) Удалить "tmpDir2".
                Directory.Delete(tmpDir2, true);

            // 6) Копировать в "TargetDirectory\archive" - с точно таким же расположением папок как и в "SourceDirectory".
            MainActions.CreateDirectory(Path.Combine(targetDirPath, "archive"));

            // ----

            string fileName = e.Name;

            RecordEntry(fileEvent, fileName, logFilePath);
        }

        // Writes information in the "log.txt" file.
        // "logFilePath" - example "D:\\log.txt".
        private void RecordEntry(string fileEvent, string fileName, string logFilePath)
        {
            // lock(obj) - is using for sync. threads.
            // obj - объект-заглушка, объект obj блокируется и работать может только один поток.
            lock (obj)
            {               
                // "true" - says that we'll append information to the end of the file.
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"[{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}] - File " +
                                     $"{fileName} {fileEvent}" );
                    writer.Flush();
                }
            }
        }

    }
}
