using System;
using System.Collections.Generic;
using System.Diagnostics;
using Data;

namespace Sources
{
    public class ProcessCountingSource : IDataSource
    {
        private MetricSpecification _spec;

        private string _processToMonitor;

        private string _processCountName;

        private int _delay;

        public int Delay
        {
            get
            {
                return _delay;
            }
        }

        public string Name
        {
            get { return _processToMonitor + "-metrics"; }
        }

        public ProcessCountingSource(string processCountFriendlyName, string processToMonitor, int delay)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;

            _delay = delay * 1000;

            _spec = new MetricSpecification(_processCountName, 0, null);
        }

        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public IEnumerable<IMetricData> Query()
        {
            var values = new Dictionary<string, double?>();

            var processes = Process.GetProcessesByName(_processToMonitor);

            values[_processCountName] = processes.Length;

            return new List<IMetricData> { new MetricData(values, DateTime.Now) };
        }
    }
}