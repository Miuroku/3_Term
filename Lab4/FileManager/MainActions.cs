using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using static System.Console;

namespace FileWatcher
{
    public static class MainActions
    {        
        // Creates new file and deletes old with the same name.
        public static void CreateFile(string path)
        {            

            if (File.Exists(path))
            {
                WriteLine($"File wasn't created !");
                WriteLine($"File {path} is already exists ! \n");
                
            }
            else 
            {
                try
                {
                    // We are using "using" cuz "File.Create()" returns stream and doesn't close it.
                    using (FileStream fs = File.Create(path))
                    {

                    }

                    WriteLine($"File \"{path}\" was successfully created ! \n");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                    return;
                }
                
            }            
        }

        // Creates new Directory and deletes old with the same name.
        public static string CreateDirectory(string path)
        {
            string ourPath = path;

            if (Directory.Exists(path))
            {
                WriteLine($"Folder wasn't created !");
                WriteLine($"Folder {path} is already exists ! \n");
            }
            else
            {
                try
                {
                    DirectoryInfo dInfo = Directory.CreateDirectory(path);

                    WriteLine($"Directory \"{path}\" was successfully created ! \n");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                    return ourPath;
                }                
            }
            return ourPath;
        }
        
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    WriteLine($"File \"{path}\" was successfully deleted ! \n");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                    return;
                }                
            }
            else
            {
                WriteLine($"File hasn't been deleted !");
                WriteLine($"File \"{path}\" doesn't exists ! \n");
            }

        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                    WriteLine($"Directory \"{path}\" was successfully deleted ! \n");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                    return;
                }                
            }
            else
            {
                WriteLine($"Folder hasn't been deleted !");
                WriteLine($"Folder \"{path}\" doesn't exists ! \n");
            }
        }

        public static string GetFilePath()
        {
            string path = "";
            string fileName = "";

            char[] badChars = { '/', '\\', ':', '*', '?', '<', '>', '"', '|' };

            Write("Enter file name : ");

            do
            {
                fileName = ReadLine();

                Write("Enter path to this file (without name) : ");

                path = ReadLine();                

                if (GoodPath(path) && GoodFileName(fileName))
                {
                    WriteLine("Your path ande name are good ! \n");
                    break;
                }
                else
                {
                    UserInteraction.InvalidInputMessage();
                }

            } while (true);

            return path;
        }

        public static string GetDirectoryPath()
        {
            string path = "";
            
            char[] badChars = { '/', '\\', ':', '*', '?', '<', '>', '"', '|' };

            Write("Enter full directory path : ");

            do
            {                
                path = ReadLine();

                if (GoodPath(path))
                {
                    WriteLine("Your path is good ! \n");
                    break;
                }
                else
                {
                    UserInteraction.InvalidInputMessage();
                }                

            } while (true);

            return path;
        }

        public static string GetFileName()
        {
            string fileName;            

            while (true)
            {
                fileName = ReadLine();

                if (GoodFileName(fileName))
                {
                    break;
                }
                else
                {
                    UserInteraction.InvalidInputMessage();
                }
            }            

            return fileName;
        }

        public static string GetFolderName()
        {
            string folderName;

            while (true)
            {
                folderName = ReadLine();

                if (GoodFolderName(folderName))
                {
                    break;
                }
                else
                {
                    UserInteraction.InvalidInputMessage();
                }
            }

            return folderName;
        }

        public static string GetFileOrFolderName()
        {
            string name;

            while (true)
            {
                name = ReadLine();

                if (GoodFileOrFolderName(name))
                {
                    break;
                }
                else
                {
                    UserInteraction.InvalidInputMessage();
                }
            }

            return name;
        }

        private static bool GoodFileName(string name)
        {
            bool niceName = true;

            char[] badNameChars = { '/', ':', '*', '?', '<', '>', '"', '|' };

            if (name.Length == 0)
                return false;

            if (!name.Contains("."))
                return false;

            foreach(var nameChar in name)
            {
                foreach (var badChar in badNameChars)
                {
                    if (nameChar == badChar)
                    {
                        return false;
                    }
                }
            }

            return niceName;
        }

        private static bool GoodPath(string path)
        {
            bool niceInput = true;

            char[] badNameChars = { '/', ':', '*', '?', '<', '>', '"', '|' };
            char[] badDiskChars = { '/', '\\', '*', '?', '<', '>', '"', '|' };

            // Stores path in convinient format.
            // [0] - drive name, [1] - ":", [2] - "\", [3] - name1, [4] - "\", [5] - name2 .. 
            List<string> separatedPath = new List<string>();

            for (int i = 0; i < path.Length; i++)
            {
                separatedPath.Add("");
            }                

            for (int i = 0, j = 0; i < path.Length; i++)
            {
                if (j == 0)
                {
                    if (path[i] == ':')
                    {
                        if (separatedPath[j].Length == 0 || !DriveExists(separatedPath[j]))
                        {
                            niceInput = false;
                            break;
                        }
                        else
                        {
                            j++;
                            separatedPath[j] = ":";
                            j++;                            
                        }

                    }                    
                    else
                    {
                        // Check if path[i] char is correct. 
                        foreach (var ourChar in badDiskChars)
                        {
                            if (ourChar == path[i])
                            {
                                niceInput = false;
                                break;
                            }
                        }
                        separatedPath[j] += path[i];
                    }
                }
                else if (j % 2 == 0)
                {
                    if (path[i] == '\\')
                    {
                        separatedPath[j] = "\\";
                        j++;
                    }
                    else
                    {
                        niceInput = false;
                        break;
                    }                    
                }
                else // Here we checks for correct name.
                {
                    if (path[i] == '\\')
                    {
                        if (separatedPath[j].Length == 0)
                        {
                            niceInput = false;
                            break;
                        }
                        else
                        {
                            j++;
                            separatedPath[j] = path[i].ToString();
                            j++;
                        }
                    }
                    else
                    {
                        foreach (var ourChar in badNameChars)
                        {
                            if (path[i] == ourChar)
                            {
                                UserInteraction.InvalidInputMessage();
                                break;
                            }
                        }
                        separatedPath[j] += path[i];                        

                    }
                }


            } // End for.

            return niceInput;
        }

        private static bool GoodFolderName(string folderName)
        {
            bool niceName = true;
            
            char[] badNameChars = { '/', ':', '*', '?', '<', '>', '"', '|'};

            if (folderName.Length == 0)
                return false;
            

            foreach (var nameChar in folderName)
            {
                foreach (var badChar in badNameChars)
                {
                    if (nameChar == badChar)
                    {
                        return false;
                    }
                }
            }

            return niceName;
        }

        // File name can contain a dot ".".
        private static bool GoodFileOrFolderName(string name)
        {
            bool niceName = true;

            char[] badNameChars = { '/', ':', '*', '?', '<', '>', '"', '|' };

            if (name.Length == 0)
                return false;


            foreach (var nameChar in name)
            {
                foreach (var badChar in badNameChars)
                {
                    if (nameChar == badChar)
                    {
                        return false;
                    }
                }
            }

            return niceName;
        }        

        // Checks if such drive exists, will it work for other OS?
        private static bool DriveExists(string driveName)
        {
            bool answ = false;

            driveName += @":\";

            try 
            {
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    if (driveName == drive.Name)
                    {
                        answ = true;
                    }
                }
            }
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);                
            }                                   

            return answ;
        }

        public static void GetFileInformation(string path)
        {
            if (!File.Exists(path))
            {
                WriteLine($"File \"{path}\" doesn't exists ! \n");
                return;
            }
            else
            {
                FileInfo fInfo = new FileInfo(path);

                WriteLine($"Name : \"{fInfo.Name}\" ");
                WriteLine($"Directory : \"{fInfo.DirectoryName}\"");
                WriteLine($"Full name : \"{fInfo.FullName}\" ");
                WriteLine($"Size : {fInfo.Length} bytes ");
                WriteLine($"Extension : {fInfo.Extension} ");
                WriteLine($"Creation time : {fInfo.CreationTime} ");
                WriteLine($"Last write time : {fInfo.LastAccessTime} ");
                WriteLine();

            }

        }

        public static void GetDirectoryInformation(string path)
        {
            if (!Directory.Exists(path))
            {
                WriteLine($"Directory \"{path}\" doesn't exists ! \n");
                return;
            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);               

                WriteLine($"Name : \"{dirInfo.Name}\" ");
                WriteLine($"Root : \"{dirInfo.Root}\" ");
                WriteLine($"Parent : \"{dirInfo.Parent}\" ");
                WriteLine($"Full name : \"{dirInfo.FullName}\" ");                                
                WriteLine($"Creation time : {dirInfo.CreationTime} ");                
                WriteLine();
            }
        }
        
        // Returns files from all directories.
        private static string[] GetAllFiles()
        {
            List<string> ourFiles = new List<string>();

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    var partOfFiles = GetAllFiles(drive.Name);

                    // Add our "partOfFiles" list to the original "ourFiles" list.
                    ourFiles.AddRange(partOfFiles);

                }
            }
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);
            }            

            return ourFiles.ToArray();
        }

        private static List<string> GetAllFiles(string dirPath)
        {
            //string[] files = Directory.GetFiles(dirPath, string.Empty, SearchOption.AllDirectories);                    

            DirectoryInfo dir_info = new DirectoryInfo(dirPath);
            List<string> file_list = new List<string>();
            ProcessDirectoriesForFiles(dir_info, file_list);

            return file_list;
        }

        private static void ProcessDirectoriesForFiles(DirectoryInfo dir_info, List<string> file_list)
        {
            try
            {
                var subDirectories = dir_info.GetDirectories();
                foreach (DirectoryInfo subdir_info in subDirectories)
                {
                    ProcessDirectoriesForFiles(subdir_info, file_list);
                }
            }
            catch
            {
            }
            try
            {
                foreach (FileInfo file_info in dir_info.GetFiles())
                {
                    file_list.Add(file_info.FullName);
                }
            }
            catch
            {
            }
        }

        // Search file in all directories.
        public static void SearchFile()
        {
            Write("Enter file name : ");

            string fileName = GetFileName();

            var allFiles = GetAllFiles();

            WriteLine("\n\tAll founded files : ");

            var amountOfFiles = 1;

            foreach (var file in allFiles)
            {                
                if (file.EndsWith(fileName))
                    WriteLine($"\n\t  File № {amountOfFiles++} : \"{file}\"");                

            }

            if (amountOfFiles == 1)
                WriteLine($"\n\t There's no such file named {fileName} ! ");

            WriteLine();
        }

        // Search file in special directory "dirPath".
        // Outputs all founded files.
        public static void SearchFile(string dirPath)
        {
            Write("Enter file name : ");

            string fileName = GetFileName();

            var allFiles = GetAllFiles(dirPath);

            WriteLine("All founded files : ");

            var amountOfFiles = 1;

            foreach (var file in allFiles)
            {
                if (file.Contains(fileName))
                    WriteLine($"File № {amountOfFiles++} : \"{file}\"");
            }

        }        

        private static string[] GetAllDirectories()
        {
            List<string> ourDirectories = new List<string>();

            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in drives)
                {
                    var partOfDirectories = GetAllDirectories(drive.Name);

                    // Add our "partOfFiles" list to the original "ourFiles" list.
                    ourDirectories.AddRange(partOfDirectories);

                }
            }
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);
            }            

            return ourDirectories.ToArray();
        }

        private static List<string> GetAllDirectories(string dirPath)
        {                               

            DirectoryInfo dir_info = new DirectoryInfo(dirPath);
            List<string> dir_list = new List<string>();
            ProcessDirectoriesForDirectories(dir_info, dir_list);

            return dir_list;
        }

        private static void ProcessDirectoriesForDirectories(DirectoryInfo dir_info, List<string> dir_list)
        {
            try
            {
                var subDirectories = dir_info.GetDirectories();
                foreach (DirectoryInfo subdir_info in subDirectories)
                {
                    dir_list.Add(subdir_info.FullName);
                    ProcessDirectoriesForDirectories(subdir_info, dir_list);
                }
            }
            catch
            {
            }           
        }
        
        public static void SearchFolder()
        {
            Write("Enter folder name : ");

            string folderName = GetFolderName();

            var allDirectories = GetAllDirectories();

            WriteLine("\n\tAll founded directories : ");

            var amountOfDirectories = 1;

            foreach (var file in allDirectories)
            {
                if (file.EndsWith(folderName))
                    WriteLine($"\n\t  File № {amountOfDirectories++} : \"{file}\"");

            }

            if (amountOfDirectories == 1)
                WriteLine($"\n\t There's no such file named {folderName} ! ");

            WriteLine();
        }
        
        private static void SearchFolder(string dirPath)
        {
            Write("Enter folder name : ");

            string folderName = GetFolderName();

            var allDirectories = GetAllDirectories(dirPath);

            WriteLine("\n\tAll founded directories : ");

            var amountOfDirectories = 1;

            foreach (var file in allDirectories)
            {
                if (file.EndsWith(folderName))
                    WriteLine($"\n\t  File № {amountOfDirectories++} : \"{file}\"");

            }

            if (amountOfDirectories == 1)
                WriteLine($"\n\t There's no such file named {folderName} ! ");

            WriteLine();

        }

        private static string GetDirectoryFromFileOrFolderPath(string filePath)
        {
            var dirPath = string.Empty;

            int indexNameStarts;

            for (indexNameStarts = (filePath.Length - 1);filePath[indexNameStarts] != '\\' ; indexNameStarts--)
            {
                
            }

            dirPath = filePath.Substring(0, indexNameStarts);     

            return dirPath;
        }

        private static string GetFileOrFolderNameFromPath(string filePath)
        {
            var fileName = string.Empty;

            for (int i = (filePath.Length - 1); filePath[i] != '\\'; i--)
            {
                fileName += filePath[i];
            }

            var charArray = fileName.ToCharArray();
            Array.Reverse(charArray);

            return new string(charArray);
        }

        public static string RenameFile(string filePath, string newFileName)
        {                        
            var newFilePath = Path.Combine(GetDirectoryFromFileOrFolderPath(filePath), newFileName);

            if (!File.Exists(filePath))
            {
                WriteLine($"Such file \"{filePath}\" doesn't exists ! \n");
            }
            else if (File.Exists(newFilePath))
            {
                WriteLine($"Such file \"{newFileName}\" already exists ! \n");
            }
            else
            {
                try
                {
                    File.Move(filePath, newFilePath);                    
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);                    
                }
                finally
                {
                    //WriteLine();
                }
            }

            return newFilePath;
        }

        public static void RenameFolder(string dirPath)
        {
            Write("Enter new folder name : ");

            var newFolderName = GetFolderName();
            var newFolderPath = Path.Combine(GetDirectoryFromFileOrFolderPath(dirPath), newFolderName);

            if (!Directory.Exists(dirPath))
            {
                WriteLine($"Such folder \"{dirPath}\" doesn't exists ! \n");
            }
            else if (Directory.Exists(newFolderPath))
            {
                WriteLine($"Such folder \"{newFolderName}\" already exists ! \n");
            }
            else
            {
                try
                {
                    Directory.Move(dirPath, newFolderPath);
                    WriteLine("Folder renamed successfully ! ");
                }
                catch(Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                }
                finally
                {
                    WriteLine();
                }
            }

        }

        // Returns new file name.
        public static string MoveFileOrFolder(string currentDirPath, string newDirPath)
        {

            var supposedNewPath = string.Empty;

            try
            {
                supposedNewPath = Path.Combine(newDirPath, GetFileOrFolderNameFromPath(currentDirPath));
            }            
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);
                return supposedNewPath;
            }

            if (!File.Exists(currentDirPath) && !Directory.Exists(currentDirPath))
            {
                WriteLine($"Such file or folder \"{currentDirPath}\" doesn't exists ! ");
            }
            else if (!Directory.Exists(newDirPath))
            {
                WriteLine($"Such file or folder \"{newDirPath}\" doesn't exists ! ");
            }
            else if (File.Exists(supposedNewPath) || Directory.Exists(supposedNewPath))
            {
                WriteLine($"Such file or directory \"{supposedNewPath}\" already exists ! ");
            }
            else
            {
                try
                {
                    Directory.Move(currentDirPath, supposedNewPath);
                    WriteLine("Move successfully ! ");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                }
                
            }

            WriteLine();

            return supposedNewPath;
        }        

        // Write text to the end of file (doesn't rewrite it).
        public static void WriteToFile(string fileName)
        {

            FileInfo file;

            try
            {
                file = new FileInfo(fileName);
            }            
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);
                WriteLine();
                return;
            }

            if (!file.Exists)
            {
                UserInteraction.ErrorMsg();
                WriteLine($"File {fileName} doesn't exists !");
            }
            else if (file.Extension != ".txt")
            {
                UserInteraction.ErrorMsg();
                WriteLine($"File {fileName} should be an .txt file !");
            }            
            else
            {
                Write("Input text (for writing to file) : ");
                var userText = ReadLine();

                try
                {

                    using (StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLineAsync(userText);
                    }

                    UserInteraction.SuccessMsg();
                    WriteLine("Recording completed !");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                }

            }            

            WriteLine();
        }

        public static void WriteToFile(string fileName, string userText)
        {

            FileInfo file;

            try
            {
                file = new FileInfo(fileName);
            }
            catch (Exception ex)
            {
                UserInteraction.PrintExceptionInfo(ex);
                WriteLine();
                return;
            }

            if (!file.Exists)
            {
                UserInteraction.ErrorMsg();
                WriteLine($"File {fileName} doesn't exists !");
            }
            else if (file.Extension != ".txt")
            {
                UserInteraction.ErrorMsg();
                WriteLine($"File {fileName} should be an .txt file !");
            }
            else
            {                

                try
                {

                    using (StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLineAsync(userText);
                    }

                    UserInteraction.SuccessMsg();
                    WriteLine("Recording completed !");
                }
                catch (Exception ex)
                {
                    UserInteraction.PrintExceptionInfo(ex);
                }

            }

            WriteLine();
        }

        public static void ReadTextFromFile(string filePath)
        {
            string stringFromFile = string.Empty;
            FileInfo file;

            try
            {
                file = new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
                WriteLine();
                return;
            }

            if (!file.Exists)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.FileNotExistsMsg(file.FullName);                
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        stringFromFile = sr.ReadToEnd();
                    }

                    UserInteraction.SuccessMsg();
                    WriteLine("Text from file : ");
                    WriteLine(stringFromFile);                  

                }
                catch (Exception ex)
                {
                    UserInteraction.ErrorMsg();
                    UserInteraction.PrintExceptionInfo(ex);                    
                }
                
            }

            WriteLine();
        }

        // Returns new file name.
        // Path to the file or folder we want to compress.
        // Compresses to ".gz" format.
        public static string Compress(string path)
        {
            string directoryOfPath = GetDirectoryFromFileOrFolderPath(path);
            string fileOrFolderNameFromPath = GetFileOrFolderNameFromPath(path);
            string newFileOrFolderNameFromPath;
            string destinationPath = string.Empty;

            try
            {
                newFileOrFolderNameFromPath = Path.ChangeExtension(fileOrFolderNameFromPath, ".gz");
                destinationPath = Path.Combine(directoryOfPath, newFileOrFolderNameFromPath);

                // Process will depend on is it file or directory.
                if (File.Exists(path))
                {
                    // поток для чтения исходного файла
                    using (FileStream sourceStream = new FileStream(path, FileMode.Open))
                    {
                        // поток для записи сжатого файла
                        using (FileStream targetStream = File.Create(destinationPath))
                        {
                            // поток архивации
                            using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                            {
                                // копируем байты из одного потока в другой
                                sourceStream.CopyTo(compressionStream); 
                                WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                                    path, sourceStream.Length.ToString(), targetStream.Length.ToString());
                            }
                        }
                    }
                }
                else if (Directory.Exists(path))
                {
                    ZipFile.CreateFromDirectory(path, destinationPath);
                }
                else
                {
                    throw new Exception($"Such Directory or file {path} doesn't exists ! ");
                }
                

                UserInteraction.SuccessMsg();
                WriteLine($"Compressed file : {destinationPath} !");
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);               
            }

            WriteLine();
            return destinationPath;
        }

        // Path of compressed file or folder. 
        // Typically : Path.ChangeExtension(dirPath, ".zip").
        // dirPath - source filepath. 
        public static void DecompressZip(string path)
        {
            string directoryOfPath = GetDirectoryFromFileOrFolderPath(path);
            string fileOrFolderNameFromPath = GetFileOrFolderNameFromPath(path);
            string newFileOrFolderNameFromPath;
            string destinationPath;

            try
            {
                newFileOrFolderNameFromPath = Path.ChangeExtension(fileOrFolderNameFromPath, null);
                destinationPath = Path.Combine(directoryOfPath, newFileOrFolderNameFromPath);

                if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                {
                    throw new Exception($"Such file or directory {destinationPath} is already exists !");
                }
                else
                {
                    if (File.Exists(path))
                    {
                        Directory.CreateDirectory(destinationPath);
                        ZipFile.ExtractToDirectory(path, destinationPath);                                               
                    }
                    else
                    {
                        throw new Exception($"Such Directory or file {path} doesn't exists ! ");
                    }
                }                

                UserInteraction.SuccessMsg();
                WriteLine($"Decompressed file : {destinationPath} !");
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }

            WriteLine();
        }

        // Returns path of decompressed file.
        // "path" - Path of compressed file or folder. 
        // Typically : Path.ChangeExtension(dirPath, ".gz").
        // dirPath - source filepath. 
        public static string DecompressGZip(string path)
        {
            string directoryOfPath = Path.GetDirectoryName(path);
            string fileOrFolderNameFromPath = Path.GetFileNameWithoutExtension(path);
            string newFileOrFolderNameFromPath;
            string destinationPath = String.Empty;

            try
            {
                newFileOrFolderNameFromPath = fileOrFolderNameFromPath + ".txt"; // А что если я заранеее не знаю какого расширения 
                // "destinationPath" - path to the decompressed file or directory.
                destinationPath = Path.Combine(directoryOfPath, newFileOrFolderNameFromPath);

                if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
                {
                    throw new Exception($"Such file or directory {destinationPath} is already exists !");
                }
                else
                {
                    // Проверяем существует ли файл котрый мы хотим распаковать. Прим : "fileOrFolder.gz".
                    if (File.Exists(path)) 
                    {
                        // Логика распаковки файла\директории ....................                        

                        // поток для чтения из сжатого файла
                        using (FileStream sourceStream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            // поток для записи восстановленного файла
                            using (FileStream targetStream = File.Create(destinationPath))
                            {
                                // поток разархивации
                                using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                                {
                                    decompressionStream.CopyTo(targetStream);
                                    //Console.WriteLine("Восстановлен файл: {0}", newFileOrFolderNameFromPath);
                                }
                            }
                        }

                    }
                    else
                    {
                        throw new Exception($"Such Directory or file {path} doesn't exists ! ");
                    }

                }
               
                UserInteraction.SuccessMsg();
                //WriteLine($"Compressed file : {destinationPath} !");
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }

            //WriteLine();
            return destinationPath;
        }

        // Пока что полностью не нужные методы ...
        public static void CheckSourceDir(string sourceDirPath, string targetDirectoryPath)
        {            

            List<string> existedFiles;
            List<string> newFiles;
            //List<string> newCompressedFiles; // Stores names of files after compressing.

            try
            {
                existedFiles = new List<string>();
                newFiles = new List<string>();
                //newCompressedFiles = new List<string>();

                DirectoryInfo dirInfo = new DirectoryInfo(sourceDirPath);

                ProcessDirectoriesForNewFiles(dirInfo, newFiles, existedFiles);

                // Processing all new files.
                foreach (var newFile in newFiles)
                {

                    // Encryption logic....
                    var encryptedFileName = Encrypt(newFile);

                    // Compress...
                    Compress(encryptedFileName);
                    //newCompressedFiles.Add(GetZipArchiveName(encryptedFileName));

                    // Move archive file to "TargetDirectory"...
                    var movedFile = MoveFileOrFolder(encryptedFileName, targetDirectoryPath);

                    // Decompress...
                    DecompressGZip(movedFile);

                    // DeEncryption...
                    Decrypt(movedFile);
                }

                // Update existed files.
                existedFiles.AddRange(newFiles);

            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }                        

            WriteLine();
        }

        // Пока что полностью не нужные методы ...
        // Chenges the list of new files "NewFiles".
        private static void ProcessDirectoriesForNewFiles(DirectoryInfo dir_info, List<string> newFiles, List<string> existedFiles)
        {
            try
            {
                var subDirectories = dir_info.GetDirectories();
                foreach (DirectoryInfo subdir_info in subDirectories)
                {
                    ProcessDirectoriesForFiles(subdir_info, newFiles);
                }
            }
            catch
            {
            }
            try
            {
                foreach (FileInfo file_info in dir_info.GetFiles())
                {
                    bool wasAdded = false;

                    foreach (var existedFile in existedFiles)
                    {
                        if (file_info.FullName == existedFile)
                        {
                            wasAdded = true;
                            break;
                        }
                    }

                    // Если файл не был в "existedFiles".
                    if (!wasAdded)
                    {
                        newFiles.Add(file_info.FullName);
                    }
                    
                }
            }
            catch
            {
            }
        }        

        public static string Encrypt(string sourceFileName)
        {
            string destinationFileName = string.Empty;
            string encryptedFileName = "encryptedVersion_"  + Path.GetFileName(sourceFileName);

            // Full path of encrypted file.
            destinationFileName = Path.Combine(Path.GetDirectoryName(sourceFileName), encryptedFileName);

            try
            {
                using (var sourceStream = File.OpenRead(sourceFileName))
                using (var destinationStream = File.Create(destinationFileName))
                using (var provider = new AesCryptoServiceProvider())
                using (var cryptoTransform = provider.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    // Save the IV to the file for decryption.
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    sourceStream.CopyTo(cryptoStream);
                    key_Aes = provider.Key;
                }
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }            

            return destinationFileName;
        }

        private static byte[] key_Aes = null;

        public static string Decrypt(string encryptedFilePath)
        {
            string decryptedFileName = String.Empty;
            string directoryPath = string.Empty;            

            try
            {
                directoryPath = Path.GetDirectoryName(encryptedFilePath);
                decryptedFileName = Path.GetFileName(encryptedFilePath).Remove(0, "encryptedVersion_".Length);
                decryptedFileName = Path.Combine(directoryPath, decryptedFileName);
                // Decrypt the source file and write it to the destination file.
                using (var sourceStream = File.OpenRead(encryptedFilePath))
                using (var destinationStream = File.Create(decryptedFileName))
                using (var provider = new AesCryptoServiceProvider())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using (var cryptoTransform = provider.CreateDecryptor(key_Aes, IV))
                    using (var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(destinationStream);
                    }
                }
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }

            return decryptedFileName;
        }

        // Renames according pattern - "Sales_YYYY_MM_DD_HH_mm_SS.txt"
        public static string RenameBuisnessFile (string sourceFilePath)
        {
            string childDirectory = Path.GetDirectoryName(sourceFilePath);
            string newFilePath = string.Empty;

            try
            {                
                FileInfo ourFile = new FileInfo(sourceFilePath);                                
                DateTime creationTime = ourFile.CreationTime;

                string newFileName = $"Q_{creationTime.Year}_{creationTime.Month}_{creationTime.Day}_{creationTime.Hour}_" +
                    $"{creationTime.Minute}_{creationTime.Second}.txt";

                // Строим директорию относительно вермени создания файла.
                childDirectory = Path.Combine(childDirectory, $"{creationTime.Year}");
                childDirectory = Path.Combine(childDirectory, $"{creationTime.Month}");
                childDirectory = Path.Combine(childDirectory, $"{creationTime.Day}");

                if (!Directory.Exists(childDirectory))
                    Directory.CreateDirectory(childDirectory);

                newFilePath = Path.Combine(childDirectory, newFileName);

                // Chenges name of file if need.
                newFilePath = GetUniqueFileName(newFilePath);

                RenameFile(sourceFilePath, newFilePath);               
            }
            catch (Exception ex)
            {
                UserInteraction.ErrorMsg();
                UserInteraction.PrintExceptionInfo(ex);
            }

            return newFilePath;
        }        

        // "sourceFolder" - isn't exists yet, so first it checks this suggestion (if "sourceFolder" is valid).
        public static string GetUniqueFolderName(string sourceFolder)
        {
            string newDirPath = sourceFolder;
            int counter = 1;

            int lengthBefore = newDirPath.Length;

            if (!(Directory.Exists(newDirPath) || File.Exists(newDirPath)))
                return newDirPath;

            newDirPath = newDirPath.Remove(lengthBefore, newDirPath.Length - lengthBefore).Insert(lengthBefore, $"({counter})");

            while (Directory.Exists(newDirPath) || File.Exists(newDirPath))
            {
                // Construct new path by inserting $"({counter})". 
                newDirPath = newDirPath.Remove(lengthBefore, newDirPath.Length - lengthBefore).Insert(lengthBefore,$"({counter})");

                counter ++;
            }

            return newDirPath;
        }        

        // Returns unique file name.
        public static string GetUniqueFileName(string sourceFile)
        {
            string newFilePath = sourceFile;
            string filePath = Path.GetDirectoryName(newFilePath);
            string nameBefore = Path.GetFileNameWithoutExtension(newFilePath);
            string extension = Path.GetExtension(newFilePath);
            int counter = 1;

            // length without extension.
            int lengthBefore = nameBefore.Length;

            if (!(Directory.Exists(newFilePath) || File.Exists(newFilePath)))
                return newFilePath;

            string tmpName = Path.GetFileNameWithoutExtension(newFilePath);

            // Adds $"({counter})".
            newFilePath = tmpName.Remove(lengthBefore, tmpName.Length - lengthBefore).Insert(lengthBefore, $"({counter})");
            newFilePath += extension;
            newFilePath = Path.Combine(filePath, newFilePath);

            while (Directory.Exists(newFilePath) || File.Exists(newFilePath))
            {
                tmpName = Path.GetFileNameWithoutExtension(newFilePath);
                // Construct new path by inserting $"({counter})". 
                newFilePath = tmpName.Remove(lengthBefore, tmpName.Length - lengthBefore).Insert(lengthBefore, $"({counter})");
                newFilePath += extension;
                newFilePath = Path.Combine(filePath, newFilePath);

                counter++;
            }

            return newFilePath;
        }

        // Creates subdirectory "dirName" in "childDirectory"
        public static string CreateSubDirectory(string childDirectory, string dirName)
        {
            string tmpDirectory = Path.Combine(childDirectory, dirName);

            // Checks if our path is available, if not, creates new and valid name.
            tmpDirectory = MainActions.GetUniqueFolderName(tmpDirectory);

            Directory.CreateDirectory(tmpDirectory);

            return tmpDirectory;
        }

    }
}
