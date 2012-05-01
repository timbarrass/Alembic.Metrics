using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class PerformanceCounterDataSource : IDataSource
    {
        private MetricSpecification[] _spec;

        private PerformanceCounter committedBytes = new PerformanceCounter { CategoryName = "Memory", CounterName = "Committed Bytes" };
        private PerformanceCounter processorTime = new PerformanceCounter { CategoryName = "Processor", CounterName = "% Processor Time", InstanceName = "_Total" };

        private string _committedName;
        private string _processorName;

        public PerformanceCounterDataSource()
        {
            _committedName = "Committed B on " + Environment.MachineName;
            _processorName = "Total CPU use on " + Environment.MachineName;

            _spec = new[]
                {
                    new MetricSpecification(_committedName, 0f, null), 
                    new MetricSpecification(_processorName, 0f, 100f),
                };
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        public IMetricData Query()
        {
            var values = new Dictionary<string, double?>
                             {
                                 { _committedName, committedBytes.NextValue() },
                                 { _processorName, processorTime.NextValue() },
                             };
            
            return new MetricData(values, DateTime.Now);
        }
    }
}