using System;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using Sinks;
using Sources;

namespace MetricAgent
{
    class Program : ServiceBase
    {
        private Agent _agent;

        private IDataSource _source;

        private IDataSink _sink;

        public Program()
        {
            var agentLoopDelay = 5;

            var outputPath = ConfigurationSettings.AppSettings["outputPath"];
            var processToMonitor = ConfigurationSettings.AppSettings["processToMonitor"];

            _source = new CompositeSource( new PerformanceCounterDataSource(), new ProcessCountingSource(processToMonitor) );

            //_sink = new RRDDataSink(_source.Spec, agentLoopDelay * 3, outputPath);
            _sink = new CircularDataSink(30, _source.Spec);

            _agent = new Agent(_source, _sink, agentLoopDelay);

            ServiceName = "Metric Agent";
        }

        static void Main(string[] args)
        {
            Console.WriteLine("This app is intended to be run as a windows service.");

            var app = new Program();

            app.OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            _agent.Start();
        }

        protected override void OnStop()
        {
            _agent.Stop();
        }
    }

    [RunInstaller(true)]
    public class MyProjectInstaller : Installer
    {
        private ServiceInstaller serviceInstaller1;
        private ServiceProcessInstaller processInstaller;

        public MyProjectInstaller()
        {
            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller1 = new ServiceInstaller();

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started manually.
            serviceInstaller1.StartType = ServiceStartMode.Manual;

            // ServiceName must equal those on ServiceBase derived classes.            
            serviceInstaller1.ServiceName = "Metric Agent";

            // Add installers to collection. Order is not important.
            Installers.Add(serviceInstaller1);
            Installers.Add(processInstaller);
        }
    }
}
