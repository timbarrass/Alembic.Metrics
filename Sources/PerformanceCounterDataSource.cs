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

        //private PerformanceCounter committedBytes = new PerformanceCounter { CategoryName = "Memory", CounterName = "Committed Bytes" };
        //private PerformanceCounter processorTime = new PerformanceCounter { CategoryName = "Processor", CounterName = "% Processor Time", InstanceName = "_Total" };

        //private string _committedName;
        //private string _processorName;

        public PerformanceCounterDataSource(string name, string categoryName, string counterName, string instanceName, float? min, float? max)
        {
            _counterName = name;

            if (string.IsNullOrEmpty(instanceName))
            {
                _counter = new PerformanceCounter {CategoryName = categoryName, CounterName = counterName};
            }
            else
            {
                _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName, InstanceName = instanceName };
            }

            //_committedName = "Committed B on " + Environment.MachineName;
            //_processorName = "Total CPU use on " + Environment.MachineName;

            _spec = new[]
                {
                    new MetricSpecification(_counterName, min, max)
                };

            //_spec = new[]
            //    {
            //        new MetricSpecification(_committedName, 0f, null), 
            //        new MetricSpecification(_processorName, 0f, 100f),
            //    };
        }

        public PerformanceCounterDataSource(string name,string categoryName, string counterName, float? min, float? max)
        {
            _counterName = name;

            _counter = new PerformanceCounter { CategoryName = categoryName, CounterName = counterName };

            //_committedName = "Committed B on " + Environment.MachineName;
            //_processorName = "Total CPU use on " + Environment.MachineName;

            _spec = new[]
                {
                    new MetricSpecification(_counterName, min, max)
                };

            //_spec = new[]
            //    {
            //        new MetricSpecification(_committedName, 0f, null), 
            //        new MetricSpecification(_processorName, 0f, 100f),
            //    };
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        public IMetricData Query()
        {
            var values = new Dictionary<string, double?>
                            {
                                { _counterName, _counter.NextValue() }
                            };
                             //{
                             //    { _committedName, committedBytes.NextValue() },
                             //    { _processorName, processorTime.NextValue() },
                             //};
            
            return new MetricData(values, DateTime.Now);
        }
    }
}