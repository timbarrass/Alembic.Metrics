using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class ProcessCountingSource : IDataSource
    {
        private ICollection<MetricSpecification> _spec;

        private string _processToMonitor;

        private string _processCountName;

        private string _processUptimeName;

        public ProcessCountingSource(string processCountFriendlyName, string processUptimeFriendlyName, string processToMonitor)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;
;
            _processUptimeName = processUptimeFriendlyName;

            _spec = new List<MetricSpecification>
                {
                    new MetricSpecification(_processCountName, 0, null),
                    new MetricSpecification(_processUptimeName, 0, null),
                };
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

            values[_processCountName] = processes.Length;
            values[_processUptimeName] = count == 0 ? 0 : uptime / count;

            return new MetricData(values, DateTime.Now);
        }
    }
}