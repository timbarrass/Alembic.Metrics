using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Data.Linq;
using System.Linq;
using System.ServiceProcess;
using Data;
using Plotters;
using Sinks;
using Sources;
using Stores;

namespace MetricAgent
{
    class Program : ServiceBase
    {
        private Agent _agent;

        private Dictionary<IDataSource, IList<IDataSink<IMetricData>>> _sinksToUpdate = new Dictionary<IDataSource, IList<IDataSink<IMetricData>>>(); 

        private List<IDataPlotter> _plotters = new List<IDataPlotter>(); 

        public Program()
        {
            var agentLoopDelay = 1;

            var store = new FileSystemDataStore<IMetricData>();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var processes = config.Sections["processCountingSource"] as ProcessCountingSourceConfiguration;

            var _processCounterSources = new List<IDataSource>();

            for (int i = 0; i < processes.Processes.Count; i++ )
            {
                var countName = processes.Processes[i].Name + " count";
                var delay = processes.Processes[i].Delay;
                var exe = processes.Processes[i].Exe;

                var source = new ProcessCountingSource(countName, exe, delay);
                var sink = new CircularDataSink<IMetricData>(600, new[] {source.Spec}, store);

                _processCounterSources.Add(source);
                if(! _sinksToUpdate.ContainsKey(source))
                {
                    _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
                }
                _sinksToUpdate[source].Add(sink);
                _plotters.Add(new SinglePlotter<IMetricData>(sink, source.Spec));
            }

            var processCounterSpecs = _processCounterSources.Select(x => x.Spec).ToArray();
            var processCounterSink = new CircularDataSink<IMetricData>(600, processCounterSpecs); 
            foreach(var source in _processCounterSources)
            {
                _sinksToUpdate[source].Add(processCounterSink);
            }
            _plotters.Add(new MultiPlotter<IMetricData>(processCounterSink, processCounterSpecs, "combined process counts"));

            var uptimeProcesses = config.Sections["processUptimeSource"] as ProcessUptimeSourceConfiguration;

            for (int i = 0; i < uptimeProcesses.Processes.Count; i++)
            {
                var uptimeName = uptimeProcesses.Processes[i].Name + " uptime";
                var delay = uptimeProcesses.Processes[i].Delay;
                var exe = uptimeProcesses.Processes[i].Exe;

                var source = new ProcessUptimeSource(uptimeName, exe, delay);
                var sink = new CircularDataSink<IMetricData>(600, new[] { source.Spec }, store);

                if (!_sinksToUpdate.ContainsKey(source))
                {
                    _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
                }
                _sinksToUpdate[source].Add(sink);
                _plotters.Add(new SinglePlotter<IMetricData>(sink, source.Spec));
            }


            var counters = config.Sections["performanceCounterSource"] as PerformanceCounterDataSourceConfiguration;

            for (int i = 0; i < counters.Counters.Count; i++)
            {
                var counterName = counters.Counters[i].Name;
     
                var source = new PerformanceCounterDataSource(
                                 counterName,
                                 counters.Counters[i].CategoryName,
                                 counters.Counters[i].CounterName,
                                 counters.Counters[i].InstanceName,
                                 counters.Counters[i].Min,
                                 counters.Counters[i].Max,
                                 counters.Counters[i].Delay
                                 );

                var sink = new CircularDataSink<IMetricData>(600, new[] { source.Spec }, store);

                if (!_sinksToUpdate.ContainsKey(source))
                {
                    _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
                }
                _sinksToUpdate[source].Add(sink);
                _plotters.Add(new SinglePlotter<IMetricData>(sink, source.Spec));
            }

            var databases = config.Sections["databaseSource"] as SqlServerDataSourceConfiguration;

            for (int i = 0; i < databases.Databases.Count; i++)
            {
                var connectionString = databases.Databases[i].ConnectionString;
                var query = databases.Databases[i].Query;
                var name = databases.Databases[i].Name;
                var delay = databases.Databases[i].Delay;

                var context = new DataContext(connectionString);
                var spec = new MetricSpecification(name, null, null);

                var source = new SqlServerDataSource(context, spec, query, delay);
                var sink = new CircularDataSink<IMetricData>(600, new[] { source.Spec }, store);

                if (!_sinksToUpdate.ContainsKey(source))
                {
                    _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
                }
                _sinksToUpdate[source].Add(sink);
                _plotters.Add(new SinglePlotter<IMetricData>(sink, source.Spec));
            }

            _agent = new Agent(_sinksToUpdate, _plotters, agentLoopDelay);

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
