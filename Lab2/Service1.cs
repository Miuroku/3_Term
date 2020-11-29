using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;

namespace FileWatcherForFun
{
    public partial class Service1 : ServiceBase
    {
        // Here we can set paths for directories.
        string sourceDirPath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\SourceDirectory";
        string targetDirPath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\TargetDirectory\FromSourceDirectory";
        string logFilePath = @"C:\Users\supay\Desktop\Labs_CS\Lab2Stuff\TargetDirectory\log.txt";

        Logger logger;

        public Service1()
        {
            InitializeComponent();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch(); // This line uncomment for debugging.

            logger = new Logger(sourceDirPath, logFilePath, targetDirPath);
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));

            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }
    }
}
