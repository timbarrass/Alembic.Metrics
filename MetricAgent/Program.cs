using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.ServiceProcess;
using Data;
using Plotters;
using Sinks;
using Sources;
using Stores;
using Writers;
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
            var agentLoopDelay = 1;

            var store = new FileSystemDataStore<IMetricData>();

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var processes = config.Sections["processCountingSource"] as ProcessCountingSourceConfiguration;

            var processCounterSources = new List<IDataSource>();

            for (int i = 0; i < processes.Processes.Count; i++)
            {
                var id = processes.Processes[i].Id;
                var countName = processes.Processes[i].Name + " count";
                var delay = processes.Processes[i].Delay;
                var exe = processes.Processes[i].Exe;
                var machineName = processes.Processes[i].MachineName;

                var source = new ProcessCountingSource(id, countName, exe, machineName, delay);

                processCounterSources.Add(source);
                if (!_sinksToUpdate.ContainsKey(source))
                {
                    _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
                }
            }

            Log.Info("Found " + processes.Processes.Count + " processes");

            var processCounterSpecs = processCounterSources.Select(x => x.Spec).ToArray();
            var processCounterSink = new CircularDataSink<IMetricData>(600, processCounterSpecs);
            foreach (var source in processCounterSources)
            {
                _sinksToUpdate[source].Add(processCounterSink);
            }
            _plotters.Add(new MultiPlotter<IMetricData>(processCounterSink, processCounterSpecs, "combined process counts"));

            Log.Info("Found " + processCounterSources.Count + " process counter sources.");

            var uptimeProcesses = config.Sections["processUptimeSource"] as ProcessUptimeSourceConfiguration;

            for (int i = 0; i < uptimeProcesses.Processes.Count; i++)
            {
                var id = uptimeProcesses.Processes[i].Id;
                var uptimeName = uptimeProcesses.Processes[i].Name + " uptime";
                var delay = uptimeProcesses.Processes[i].Delay;
                var exe = uptimeProcesses.Processes[i].Exe;
                var machineName = uptimeProcesses.Processes[i].MachineName;

                var source = new ProcessUptimeSource(id, uptimeName, exe, machineName, delay);

                _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
            }

            Log.Info("Found " + uptimeProcesses.Processes.Count + " process uptime sources.");

            var counters = config.Sections["performanceCounterSource"] as PerformanceCounterDataSourceConfiguration;

            for (int i = 0; i < counters.Counters.Count; i++)
            {
                var id = counters.Counters[i].Id;
                var counterName = counters.Counters[i].Name;

                var source = new PerformanceCounterDataSource(
                    id,
                    counterName,
                    counters.Counters[i].CategoryName,
                    counters.Counters[i].CounterName,
                    counters.Counters[i].InstanceName,
                    counters.Counters[i].MachineName,
                    counters.Counters[i].Min,
                    counters.Counters[i].Max,
                    counters.Counters[i].Delay
                    );

                _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
            }

            var databases = config.Sections["databaseSource"] as SqlServerDataSourceConfiguration;

            for (int i = 0; i < databases.Databases.Count; i++)
            {
                var id = databases.Databases[i].Id;
                var connectionString = databases.Databases[i].ConnectionString;
                var query = databases.Databases[i].Query;
                var name = databases.Databases[i].Name;
                var delay = databases.Databases[i].Delay;

                var context = new DataContext(connectionString);
                var spec = new MetricSpecification(name, null, null);

                var source = new SqlServerDataSource(id, context, spec, query, delay);

                _sinksToUpdate[source] = new List<IDataSink<IMetricData>>();
            }

            var sinks = config.Sections["sinks"] as CircularDataSinkConfiguration;

            for (int i = 0; i < sinks.Sinks.Count; i++)
            {
                var id = sinks.Sinks[i].Id;
                var points = sinks.Sinks[i].Points;
                var directory = sinks.Sinks[i].OutputPath;

                var sources = _sinksToUpdate.Where(x => x.Key.Id.Equals(id));

                foreach (var source in sources)
                {
                    var sink = new CircularDataSink<IMetricData>(points, new[] {source.Key.Spec});

                    if (!_sinksToUpdate.ContainsKey(source.Key))
                    {
                        _sinksToUpdate[source.Key] = new List<IDataSink<IMetricData>>();
                    }

                    _sinksToUpdate[source.Key].Add(sink);
                    _plotters.Add(new SinglePlotter<IMetricData>(directory, sink, source.Key.Spec));
                    _writers.Add(new SingleWriter<IMetricData>(directory, sink, source.Key.Spec, store));
                }
            }

            _agent = new Agent(_sinksToUpdate, _plotters, _writers, agentLoopDelay);
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

            _agent.Start();
        }

        protected override void OnStop()
        {
            _agent.Stop();

            Log.Info("========================================================================");
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).Name);

        private Agent _agent;

        private Dictionary<IDataSource, IList<IDataSink<IMetricData>>> _sinksToUpdate = new Dictionary<IDataSource, IList<IDataSink<IMetricData>>>();

        private List<IDataPlotter> _plotters = new List<IDataPlotter>();

        private List<IDataWriter> _writers = new List<IDataWriter>();

        private List<IDataSink<IMetricData>> _sinks = new List<IDataSink<IMetricData>>(); 
    }
}
