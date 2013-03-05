using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Data;
using Plotters;
using Sinks;
using Sources;
using log4net;

namespace MetricAgent
{
    class Program : ServiceBase
    {
        public Program()
        {
            ServiceName = "Metric Agent";

            CanHandlePowerEvent = false;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = false;
            CanShutdown = true;
            CanStop = true;
        }

        private void Configure()
        {
            
        }

        static void Main(string[] args)
        {
            Log.Info("************************************************************************");

            var service = new Program();

            service.Configure();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Running as console app ... press a key to stop");
                Console.Read();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        }

        protected override void OnStop()
        {
            Log.Info("========================================================================");
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).Name);
    }
}
