using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class PerformanceCounterDataSource : IDataSource
    {
        private MetricSpecification[] _spec;

        private PerformanceCounter _counter;

        private string _counterName;

        private int _delay;

        public int Delay
        {
            get
            {
                return _delay;
            }
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

            _spec = new[]
                {
                    new MetricSpecification(_counterName, min, max)
                };
        }

        public PerformanceCounterDataSource(string name,string categoryName, string counterName, float? min, float? max)
        {
            _counterName = name;

            _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName };

            _spec = new[]
                {
                    new MetricSpecification(_counterName, min, max)
                };
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        public IEnumerable<IMetricData> Query()
        {
            var values = new Dictionary<string, double?>
                            {
                                { _counterName, _counter.NextValue() }
                            };

            return new List<IMetricData> { new MetricData(values, DateTime.Now) };
        }
    }
}