# 3_Term
Descripton (simple, why not) : 
1) Lab2 :
       Very sinmple : 
       We has two folders : "SourceDirectory"(imagine it's client) and "TargetDirectory"(imagine it's our server).
       Imagine the task is safely sent files from client to server. (p.s. this app doesn't use network, only files on your comuter)
       
       Program is just a Windows-service, It's monitoring one folder called "SourceDirectory" for creating a new files.
       If file created : 
         1) program will rename it by pattern - Q_YYYY_MM_DD_HH_mm_SS.txt (year, month, day, hour, seconds - file creating time)
            and construct special directories by pattern SourceDirectory/YYYY/MM/DD/ and move file there.
         2) Encrypt file.
         3) Compress file. (".gz")
         4) Move file encrypted-compressed-file to "TargetDirectory\FromSourceDirectory" and to "TargetDirectory\archive".
         5) Decompress file.
         6) Decrypt file.
       
