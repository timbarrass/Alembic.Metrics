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

        private int _delay;

        public int Delay
        {
            get
            {
                return _delay;
            }
        }

        public ProcessCountingSource(string processCountFriendlyName, string processUptimeFriendlyName, string processToMonitor, int delay)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;
;
            _processUptimeName = processUptimeFriendlyName;

            _delay = delay * 1000;

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

        public IEnumerable<IMetricData> Query()
        {
            var values = new Dictionary<string, double?>();

            var processes = Process.GetProcessesByName(_processToMonitor);

            var count = 0;
            var uptime = 0d;
            foreach(var process in processes)
            {
                try
                {
                    uptime += new TimeSpan(DateTime.Now.Ticks - process.StartTime.Ticks).TotalSeconds;
                }
                catch(InvalidOperationException ex)
                {
                    // _assume_ this is because the process has gone away between getting the process 
                    // list and making the query. Ignore.
                }

                count++;
            }

            values[_processCountName] = processes.Length;
            values[_processUptimeName] = count == 0 ? 0 : uptime / count;

            return new List<IMetricData> { new MetricData(values, DateTime.Now) };
        }
    }
}