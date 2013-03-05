using System;
using System.Diagnostics;
using Data;
using log4net;

namespace Sources
{
    public class ProcessCountingSource : ISnapshotProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcessCountingSource).Name);

        private MetricSpecification _spec;

        private string _processToMonitor;

        private string _processCountName;

        private int _delay;

        private string _id;

        private string _machineName = Environment.MachineName;

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

        public ProcessCountingSource(string id, string processCountFriendlyName, string processToMonitor, string machine, int delay)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;

            _delay = delay * 1000;

            _id = id;

            _machineName = machine;

            _spec = new MetricSpecification(_processCountName, 0, null);
        }

        public MetricSpecification Spec
        {
            get { return _spec; }
        }

        public Snapshot Snapshot()
        {
            Log.Debug("Querying " + Name);

            Process[] processes;

            if (string.IsNullOrEmpty(_machineName))
            {
                processes = Process.GetProcessesByName(_processToMonitor);
            }
            else
            {
                processes = Process.GetProcessesByName(_processToMonitor, _machineName);
            }

            return new Snapshot { new MetricData( processes.Length, DateTime.Now) };
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}