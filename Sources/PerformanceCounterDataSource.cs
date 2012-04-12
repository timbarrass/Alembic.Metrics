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
            //foreach(var process in Process.GetProcesses())
            //{
            //    Console.WriteLine(process);
            //}

            var values = new Dictionary<string, double?>
                             {
                                 { "Committed", committedBytes.NextValue() },
                                 { "Processor", processorTime.NextValue() },
                                 { "Processes", Process.GetProcessesByName("chrome").Length },
                             };
            
            return new MetricData(values, DateTime.Now);
        }
    }
}