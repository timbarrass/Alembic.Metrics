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
            var agentLoopDelay = 1;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var processes = config.Sections["processCountingSource"] as ProcessCountingSourceConfiguration;
            var countName = processes.Processes[0].Name + " count";
            var uptimeName = processes.Processes[0].Name + " uptime";
            var exe = processes.Processes[0].Exe;
            _source = new CompositeSource(new PerformanceCounterDataSource(), new ProcessCountingSource(countName, uptimeName, exe));

            for (int i = 1; i < processes.Processes.Count; i++ )
            {
                countName = processes.Processes[i].Name + " count";
                uptimeName = processes.Processes[i].Name + " uptime";
                exe = processes.Processes[i].Exe;

                _source = new CompositeSource(_source, new ProcessCountingSource(countName, uptimeName, exe));
            }

            var outputPath = ConfigurationSettings.AppSettings["outputPath"];

            _sink = new CircularDataSink(600, _source.Spec);

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
