using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class ProcessCountingSource : IDataSource
    {
        private static readonly ICollection<MetricSpecification> _spec = new List<MetricSpecification>
                                                                             {
                                                                                 new MetricSpecification("Processes", 0, null),
                                                                                 new MetricSpecification("Uptime", 0, null),
                                                                             };

        private string _processToMonitor;

        public ProcessCountingSource(string processToMonitor)
        {
            _processToMonitor = processToMonitor;
        }

        public ICollection<MetricSpecification> Spec
        {
            get { return _spec; }
        }

        public IMetricData Query()
        {
            var values = new Dictionary<string, double?>();

            var processes = Process.GetProcessesByName(_processToMonitor);

            var count = 0;
            var uptime = 0d;
            foreach(var process in processes)
            {
                uptime += new TimeSpan(DateTime.Now.Ticks - process.StartTime.Ticks).TotalSeconds;
                count++;
            }

            values["Processes"] = processes.Length;
            values["Uptime"] = count == 0 ? 0 : uptime / count;

            return new MetricData(values, DateTime.Now);
        }
    }
}