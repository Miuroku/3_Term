# Labs on c# for the University

**Descripton (simple, why not) :** 
1) Lab2 :<br />       
       We has two folders : "SourceDirectory"(imagine it's client) and "TargetDirectory"(imagine it's our server).<br />
       Imagine the task is safely sent files from client to server. (p.s. this app doesn't use network, only files on your comuter)<br />
       <br />
       Program is just a Windows-service, It's monitoring one folder called "SourceDirectory" for creating a new files.<br />
       If file created :<br />
         1) program will rename it by pattern - Q_YYYY_MM_DD_HH_mm_SS.txt (year, month, day, hour, seconds - file creating time)<br />
             and construct special directories by pattern SourceDirectory/YYYY/MM/DD/ and move file there.<br />
         2) Encrypt file.<br />
         3) Compress file. (".gz")<br />
         4) Move file encrypted-compressed-file to "TargetDirectory\FromSourceDirectory" and to "TargetDirectory\archive".<br />
         5) Decompress file.<br />
         6) Decrypt file.<br />
<br />
2) Lab3 :<br />
       The same as Lab2. BUT now we could tune such app options as :<br />
       source, target, archive and log.txt directories, encryption, logging and archivation.<br />
       Internal structure (configuration classes/interfaces): <br />
       AllOptions - represents options that we could change;<br />
       IConfigurationParser - contains one method (Parse());<br />
       JsonParser & XMLParser - universal parsers (implement IConfigurationParser);<br />
       ConfigurationLoader - gets the path and searches there for configuration files,<br />
                       and verify if all options was loaded, if not, inserts default values;<br />
       ConfigurationProvider - returns maximum options for "AllOptions".<br />      
