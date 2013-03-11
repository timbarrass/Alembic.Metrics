using System;
using System.Diagnostics;
using Data;
using log4net;

namespace Sources
{
    public class PerformanceCounterDataSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PerformanceCounterDataSource).Name);

        private MetricSpecification _spec;

        private PerformanceCounter _counter;

        private string _counterName;

        private string _id;

        public string Name { get { return _counterName; } }

        public string Id { get { return _id; } }

        public PerformanceCounterDataSource(string id, string name, string categoryName, string counterName, string instanceName, string machine, float? min, float? max) 
            : this(name, categoryName, counterName, instanceName, machine, min, max)
        {
            _id = id;
        }

        public PerformanceCounterDataSource(string name, string categoryName, string counterName, string instanceName, string machine, float? min, float? max)
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

            _spec = new MetricSpecification(_counterName, min, max);
        }

        public PerformanceCounterDataSource(string id, string name,string categoryName, string counterName, float? min, float? max)
        {
            _id = id;

            _counterName = name;

            _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName };

            _spec = new MetricSpecification(_counterName, min, max);
        }

        public PerformanceCounterDataSource(CounterElement config)
            : this(config.Id, config.Name, config.CategoryName, config.CounterName, config.InstanceName, config.MachineName, config.Min, config.Max)
        {
        }


        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public Snapshot Snapshot()
        {
            Log.Debug("Querying " + Name);

            return new Snapshot { new MetricData( _counter.NextValue(), DateTime.Now) };
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}