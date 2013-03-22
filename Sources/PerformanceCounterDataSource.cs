using System;
using System.Diagnostics;
using Data;
using log4net;

namespace Sources
{
    public class PerformanceCounterDataSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PerformanceCounterDataSource).Name);

        private PerformanceCounter _counter;

        private string _counterName;

        public string Name { get { return _counterName; } }

        public PerformanceCounterDataSource(string id, string name, string categoryName, string counterName, string instanceName, string machine) 
            : this(name, categoryName, counterName, instanceName, machine)
        {
        }

        public PerformanceCounterDataSource(string name, string categoryName, string counterName, string instanceName, string machine)
        {
            _counterName = name;

            if (string.IsNullOrEmpty(machine))
                machine = Environment.MachineName;

            if (string.IsNullOrEmpty(instanceName))
            {
                _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName, MachineName = machine };
            }
            else
            {
                _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName, InstanceName = instanceName, MachineName = machine };
            }
        }

        public PerformanceCounterDataSource(string name,string categoryName, string counterName)
        {
            _counterName = name;

            _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName };
        }

        public PerformanceCounterDataSource(CounterElement config)
            : this(config.Name, config.CategoryName, config.CounterName, config.InstanceName, config.MachineName)
        {
        }

        public Snapshot Snapshot()
        {
            Log.Debug("Querying " + Name);

            var snapshot = new Snapshot { Name = Name };
            snapshot.Add(new MetricData( _counter.NextValue(), DateTime.Now));

            return snapshot;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}