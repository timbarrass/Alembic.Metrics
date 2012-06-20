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

        private string _id;

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

        public string Id
        {
            get { return _id;  }
        }

        public ProcessCountingSource(string id, string processCountFriendlyName, string processToMonitor, int delay)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;

            _delay = delay * 1000;

            _id = id;

            _spec = new MetricSpecification(_processCountName, 0, null);
        }

        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public IEnumerable<IMetricData> Query()
        {
            var processes = Process.GetProcessesByName(_processToMonitor);

            return new List<IMetricData> { new MetricData( processes.Length, DateTime.Now) };
        }
    }
}