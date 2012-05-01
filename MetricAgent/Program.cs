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

            _source = new NullSource();

            var processes = config.Sections["processCountingSource"] as ProcessCountingSourceConfiguration;
            
            for (int i = 0; i < processes.Processes.Count; i++ )
            {
                var countName = processes.Processes[i].Name + " count";
                var uptimeName = processes.Processes[i].Name + " uptime";
                var exe = processes.Processes[i].Exe;

                _source = new CompositeSource(_source, new ProcessCountingSource(countName, uptimeName, exe));
            }

            var counters = config.Sections["performanceCounterSource"] as PerformanceCounterDataSourceConfiguration;

            for (int i = 0; i < counters.Counters.Count; i++)
            {
                var counterName = counters.Counters[i].CategoryName + "-" + counters.Counters[i].CounterName; // todo: config section needs a friendly name
                _source = new CompositeSource(_source, new PerformanceCounterDataSource(
                    counterName, 
                    counters.Counters[i].CategoryName, 
                    counters.Counters[i].CounterName, 
                    counters.Counters[i].InstanceName,
                    null, null));                
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
