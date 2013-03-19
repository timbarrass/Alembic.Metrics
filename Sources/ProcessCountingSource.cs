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

        private string _id;

        private string _machineName = Environment.MachineName;

        public string Name
        {
            get { return _processCountName; }
        }

        public string Id
        {
            get { return _id;  }
        }

        public ProcessCountingSource(ProcessElement config)
            : this(config.Id, config.Name, config.Exe, config.MachineName)
        {
        }

        public ProcessCountingSource(string id, string processCountFriendlyName, string processToMonitor, string machine)
        {
            _processToMonitor = processToMonitor;

            _processCountName = processCountFriendlyName;

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

            var snapshot = new Snapshot { Name = Name };
            snapshot.Add(new MetricData( processes.Length, DateTime.Now));

            return snapshot;
        }

        public Snapshot Snapshot(DateTime cutoff)
        {
            throw new NotImplementedException();
        }
    }
}