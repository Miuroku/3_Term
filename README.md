# Labs on c# for the University

**Descripton (simple, why not) :** 

3) Lab4 : <br/>
       What was added:<br/>
       1) XmlGenerator - generates .xml file from List<PersonInfo><br/>
       2) DataAccessLayer - base class for getting data from database<br/>
              using method T GetPersonOpt that use stored procedures<br/>
       3) ServiceLayer - using "DataAccessLayer" for getting all necessary data from DB<br/>
       4) Models - models of data, represents necessary entities from DB<br/>
       5) Converter - all-purpose Xml and Json parser<br/>
       6) Logger - write logs into the table "Log" (writes to DB)<br/>
       7) FileManager - windows service from last lab. works<br/>
       8) Main (DataManager) - firstly loads options using OptionsManager,<br/>
              than getting info from DB using ServiceLayer and generates Xml file in the SourceDirectory<br/>

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

2) Lab3 :<br />
    The same as Lab2. BUT now we could tune such app options as :<br />
    source, target, archive and log.txt directories, encryption, logging and archivation.<br />
    Internal structure (configuration classes/interfaces): <br />
    - AllOptions - represents options that we could change;<br />
    - IConfigurationParser - contains one method (Parse());<br />
    - JsonParser & XMLParser - universal parsers (implement IConfigurationParser);<br />
    - ConfigurationLoader - gets the path and searches there for configuration files,<br />
                   and verify if all options was loaded, if not, inserts default values;<br />
    - ConfigurationProvider - returns maximum options for "AllOptions".<br />      
