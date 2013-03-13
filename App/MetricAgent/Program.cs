﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Coordination;
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

            var configuration = ConfigurationManager.OpenExeConfiguration("MetricAgent.exe");

            var schedules = ConfigurationParser.Parse(configuration);

            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            var tasks = new List<Task>();

            foreach (var schedule in schedules)
            {
                var currentSchedule = schedule;

                tasks.Add(Task.Factory.StartNew(() => currentSchedule.Start(_cancellationToken), _cancellationToken));
            }

            try
            {
                Task.WaitAny(tasks.ToArray());
            }
            catch (AggregateException ae)
            {
                ae.Handle(x => { Log.Error(ae.Message); return false; });
            }
        }

        protected override void OnStop()
        {
            _tokenSource.Cancel();

            Log.Info("========================================================================");
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).Name);

        private CancellationToken _cancellationToken;

        private CancellationTokenSource _tokenSource;
    }
}
