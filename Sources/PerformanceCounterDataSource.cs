using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class PerformanceCounterDataSource : IDataSource
    {
        private static readonly MetricSpecification[] _spec = new[]
                                                                  {
                                                                      new MetricSpecification("Committed", 0f, null), 
                                                                      new MetricSpecification("Processor", 0f, 100f),
                                                                  };

        private PerformanceCounter committedBytes = new PerformanceCounter { CategoryName = "Memory", CounterName = "Committed Bytes" };
        private PerformanceCounter processorTime = new PerformanceCounter { CategoryName = "Processor", CounterName = "% Processor Time", InstanceName = "_Total" };

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        public IMetricData Query()
        {
            var values = new Dictionary<string, double?>
                             {
                                 { "Committed", committedBytes.NextValue() },
                                 { "Processor", processorTime.NextValue() },
                             };
            
            return new MetricData(values, DateTime.Now);
        }
    }
}