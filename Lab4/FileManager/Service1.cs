using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;

namespace FileWatcher
{
    public partial class Service1 : ServiceBase
    {
        Watcher watcher;

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

            watcher = new Watcher();
            Thread loggerThread = new Thread(new ThreadStart(watcher.Start));

            loggerThread.Start();
        }

        protected override void OnStop()
        {
            watcher.Stop();
            Thread.Sleep(1000);
        }
    }
}
