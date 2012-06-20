using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class PerformanceCounterDataSource : IDataSource
    {
        private MetricSpecification _spec;

        private PerformanceCounter _counter;

        private string _counterName;

        private int _delay;

        private string _id;

        public int Delay
        {
            get
            {
                return _delay;
            }
        }

        public string Name { get { return _counterName; } }

        public string Id { get { return _id; } }

        public PerformanceCounterDataSource(string id, string name, string categoryName, string counterName, string instanceName, float? min, float? max, int delay) 
            : this(name, categoryName, counterName, instanceName, min, max, delay)
        {
            _id = id;
        }

        public PerformanceCounterDataSource(string name, string categoryName, string counterName, string instanceName, float? min, float? max, int delay)
        {
            _delay = delay * 1000;

            _counterName = name;

            if (string.IsNullOrEmpty(instanceName))
            {
                _counter = new PerformanceCounter {CategoryName = categoryName, CounterName = counterName};
            }
            else
            {
                _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName, InstanceName = instanceName };
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


        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public IEnumerable<IMetricData> Query()
        {
            return new List<IMetricData> { new MetricData( _counter.NextValue(), DateTime.Now) };
        }
    }
}